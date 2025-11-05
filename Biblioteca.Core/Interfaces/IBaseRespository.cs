using Biblioteca.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biblioteca.Core.Entities;

namespace Biblioteca.Core.Interfaces;

public interface IBaseRepository<T> where T : class
{

    Task<T?> GetById(int id);
    Task Add(T entity);
    void Update(T entity);
    Task Delete(int id);
}
