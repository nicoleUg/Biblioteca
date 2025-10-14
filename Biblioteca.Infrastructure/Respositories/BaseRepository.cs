using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Biblioteca.Core.Entities;
using Biblioteca.Core.Interfaces;
using Biblioteca.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Infrastructure.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
{
    private readonly BibliotecaContext _context;
    private readonly DbSet<T> _entities;
    public BaseRepository(BibliotecaContext context)
    { _context = context; _entities = context.Set<T>(); }

    public async Task<IEnumerable<T>> GetAll() => await _entities.ToListAsync();
    public async Task<T?> GetById(int id) => await _entities.FindAsync(id);
    public async Task Add(T entity) { _entities.Add(entity); await _context.SaveChangesAsync(); }
    public async Task Update(T entity) { _entities.Update(entity); await _context.SaveChangesAsync(); }
    public async Task Delete(int id)
    {
        var entity = await GetById(id);
        if (entity is null) return;
        _entities.Remove(entity);
        await _context.SaveChangesAsync();
    }
}