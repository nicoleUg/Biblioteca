using System.Collections.Generic;
using System.Threading.Tasks;
using Biblioteca.Core.Entities;
using Biblioteca.Core.Interfaces;
using Biblioteca.Infrastructure.Data;

namespace Biblioteca.Infrastructure.Repositories
{
    public class LibroRepository : BaseRepository<Libro>, ILibroRepository
    {
        private readonly IDapperContext _dapper;
        public LibroRepository(BibliotecaContext ctx, IDapperContext dapper) : base(ctx) { _dapper = dapper; }

        public Task<IEnumerable<Libro>> GetAllDapperAsync(string? titulo, string? autor, string? categoria, bool? habilitado)
            => _dapper.QueryAsync<Libro>(
                @"SELECT Id,Titulo,Autor,Categoria,CostoReposicionBs,TotalEjemplares,EjemplaresDisponibles,CondicionGeneral,Habilitado
              FROM Libro
              WHERE (@Titulo IS NULL OR Titulo LIKE CONCAT('%',@Titulo,'%'))
                AND (@Autor IS NULL OR Autor LIKE CONCAT('%',@Autor,'%'))
                AND (@Categoria IS NULL OR Categoria = @Categoria)
                AND (@Habilitado IS NULL OR Habilitado = @Habilitado)
              ORDER BY Id DESC;",
                new { Titulo = titulo, Autor = autor, Categoria = categoria, Habilitado = habilitado });

        public Task<Libro?> GetByIdDapperAsync(int id)
            => _dapper.QueryFirstOrDefaultAsync<Libro>(
                @"SELECT Id,Titulo,Autor,Categoria,CostoReposicionBs,TotalEjemplares,EjemplaresDisponibles,CondicionGeneral,Habilitado 
              FROM Libro WHERE Id=@Id;", new { Id = id });
    }
}
