using System.Collections.Generic;
using System.Threading.Tasks;
using Biblioteca.Core.Entities;
using Biblioteca.Core.Interfaces;
using Biblioteca.Infrastructure.Data;

namespace Biblioteca.Infrastructure.Repositories;

public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
{
    private readonly IDapperContext _dapper;
    public UsuarioRepository(BibliotecaContext ctx, IDapperContext dapper) : base(ctx) { _dapper = dapper; }

    public Task<IEnumerable<Usuario>> GetAllDapperAsync(string? nombre, string? email, string? rol, bool? activo)
        => _dapper.QueryAsync<Usuario>(
            @"SELECT Id,Nombre,Email,Rol,Activo
              FROM Usuario
              WHERE (@Nombre IS NULL OR Nombre LIKE CONCAT('%',@Nombre,'%'))
                AND (@Email  IS NULL OR Email  LIKE CONCAT('%',@Email,'%'))
                AND (@Rol    IS NULL OR Rol = @Rol)
                AND (@Activo IS NULL OR Activo = @Activo)
              ORDER BY Id DESC;",
            new { Nombre = nombre, Email = email, Rol = rol, Activo = activo });

    public Task<Usuario?> GetByIdDapperAsync(int id)
        => _dapper.QueryFirstOrDefaultAsync<Usuario>(
            @"SELECT Id,Nombre,Email,Rol,Activo FROM Usuario WHERE Id=@Id;", new { Id = id });
}
