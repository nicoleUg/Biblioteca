using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Core.Entities;

/// <summary>
/// Representa la clase base para las entidades del sistema
/// </summary>
/// <remarks>
/// Esta clase abstracta define la estructura común (como el identificador) 
/// que compartirán todas las entidades persistentes de la aplicación.
/// </remarks>
public abstract class BaseEntity
{
    /// <summary>
    /// Identificador único de la entidad (Clave Primaria)
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }
}