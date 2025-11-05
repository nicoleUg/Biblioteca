using Biblioteca.Core.Entities;
using Biblioteca.Core.Interfaces;
using Biblioteca.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace Biblioteca.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BibliotecaContext _context;
        private readonly IDapperContext _dapperContext;

        private IBaseRepository<Libro>? _libros;
        private IBaseRepository<Usuario>? _usuarios;
        private IBaseRepository<Prestamo>? _prestamos;
        private IBaseRepository<Multa>? _multas;

        private ILibroRepository? _librosEx;
        private IUsuarioRepository? _usuariosEx;
        private IPrestamoRepository? _prestamosEx;
        private IMultaRepository? _multasEx;

        private IDbContextTransaction? _transaction;

        public UnitOfWork(BibliotecaContext context, IDapperContext dapperContext)
        {
            _context = context;
            _dapperContext = dapperContext;
        }

        // Repositorios base genéricos
        public IBaseRepository<Libro> Libros =>
            _libros ??= new BaseRepository<Libro>(_context);

        public IBaseRepository<Usuario> Usuarios =>
            _usuarios ??= new BaseRepository<Usuario>(_context);

        public IBaseRepository<Prestamo> Prestamos =>
            _prestamos ??= new BaseRepository<Prestamo>(_context);

        public IBaseRepository<Multa> Multas =>
            _multas ??= new BaseRepository<Multa>(_context);

        // Repositorios extendidos con métodos específicos
        public ILibroRepository LibrosEx =>
            _librosEx ??= new LibroRepository(_context, _dapperContext);

        public IUsuarioRepository UsuariosEx =>
            _usuariosEx ??= new UsuarioRepository(_context, _dapperContext);

        public IPrestamoRepository PrestamosEx =>
            _prestamosEx ??= new PrestamoRepository(_context);

        public IMultaRepository MultasEx =>
            _multasEx ??= new MultaRepository(_context);

        // Métodos de transacción
        public async Task BeginTransactionAsync()
        {
            if (_transaction == null)
            {
                _transaction = await _context.Database.BeginTransactionAsync();
            }
        }

        public async Task CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        // Métodos para Dapper
        public IDbConnection? GetDbConnection()
        {
            return _context.Database.GetDbConnection();
        }

        public IDbTransaction? GetDbTransaction()
        {
            return _transaction?.GetDbTransaction();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }
    }
}