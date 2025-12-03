using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Biblioteca.Core.DTOs;
using Biblioteca.Core.Entities;
using Biblioteca.Core.Exceptions;
using Biblioteca.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Biblioteca.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _uow;
        private readonly IConfiguration _configuration;

        public AuthService(IUnitOfWork uow, IConfiguration configuration)
        {
            _uow = uow;
            _configuration = configuration;
        }

        /// <summary>
        /// Registra un usuario nuevo: valida email, genera hash, guarda y devuelve token.
        /// </summary>
        public async Task<AuthResponseDto> RegisterAsync(AuthRegisterDto dto)
        {
            // 1) Validar que el email no esté repetido
            // GetAll es SINCRÓNICO en tu BaseRepository, así que NO lleva await
            var usuarios = await _uow.Usuarios.GetAll();
            var existente = usuarios.FirstOrDefault(u => u.Email == dto.Email);

            if (existente != null)
                throw new BusinessException("El email ya está registrado", 400);

            // 2) Crear entidad usuario
            var user = new Usuario
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                Rol = string.IsNullOrWhiteSpace(dto.Rol) ? "estudiante" : dto.Rol,
                Activo = true,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            // 3) Guardar en transacción
            await _uow.BeginTransactionAsync();
            try
            {
                await _uow.Usuarios.Add(user);
                await _uow.CommitAsync();
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }

            // 4) Generar token
            var token = GenerateToken(user);

            return new AuthResponseDto
            {
                Id = user.Id,
                Nombre = user.Nombre,
                Email = user.Email,
                Rol = user.Rol,
                Activo = user.Activo,
                Token = token
            };
        }

        /// <summary>
        /// Valida credenciales de login y genera JWT si son correctas.
        /// </summary>
        public async Task<AuthResponseDto> LoginAsync(AuthLoginDto dto)
        {
            // Igual acá: GetAll sin await
            var usuarios = await _uow.Usuarios.GetAll();
            var user = usuarios.FirstOrDefault(u => u.Email == dto.Email);

            if (user is null || string.IsNullOrWhiteSpace(user.PasswordHash) ||
                !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                throw new BusinessException("Credenciales inválidas", 400);
            }

            if (!user.Activo)
                throw new BusinessException("Usuario inactivo", 400);

            var token = GenerateToken(user);

            return new AuthResponseDto
            {
                Id = user.Id,
                Nombre = user.Nombre,
                Email = user.Email,
                Rol = user.Rol,
                Activo = user.Activo,
                Token = token
            };
        }

        private string GenerateToken(Usuario user)
        {
            var authSection = _configuration.GetSection("Authentication");
            var secretKey = authSection["SecretKey"] ?? throw new Exception("JWT SecretKey no configurado");
            var issuer = authSection["Issuer"];
            var audience = authSection["Audience"];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("nombre", user.Nombre),
                new Claim(ClaimTypes.Role, user.Rol)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
