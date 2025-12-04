using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Core.Entities;

/// <summary>
/// Representa un Libro en el sistema
/// </summary>
/// <remarks>
/// Esta entidad almacena la información bibliográfica y de inventario de cada obra,
/// sirviendo como base para las operaciones de préstamo y control de stock.
/// </remarks>
public class Libro : BaseEntity
{
    /// <summary>
    /// Título completo de la obra
    /// </summary>
    /// <example>Cien años de soledad</example>
    public string Titulo { get; set; } = null!;

    /// <summary>
    /// Nombre del autor o autores de la obra
    /// </summary>
    /// <example>Gabriel García Márquez</example>
    public string Autor { get; set; } = null!;

    /// <summary>
    /// Categoría o género literario al que pertenece el libro
    /// </summary>
    /// <example>Novela</example>
    public string Categoria { get; set; } = null!;

    /// <summary>
    /// Costo monetario de reposición en Bolivianos en caso de pérdida o daño total
    /// </summary>
    /// <example>150.00</example>
    public decimal CostoReposicionBs { get; set; }

    /// <summary>
    /// Cantidad total de ejemplares físicos registrados en el inventario
    /// </summary>
    /// <example>5</example>
    public int TotalEjemplares { get; set; }

    /// <summary>
    /// Cantidad actual de ejemplares disponibles para ser prestados
    /// </summary>
    /// <remarks>
    /// Este valor disminuye con cada préstamo y aumenta con cada devolución.
    /// </remarks>
    /// <example>3</example>
    public int EjemplaresDisponibles { get; set; }

    /// <summary>
    /// Indica si el libro está habilitado para realizar préstamos
    /// </summary>
    /// <remarks>
    /// Si es false, el libro no puede ser prestado aunque haya stock.
    /// </remarks>
    /// <example>true</example>
    public bool Habilitado { get; set; } = true;

    /// <summary>
    /// Estado físico general de los ejemplares
    /// </summary>
    /// <remarks>
    /// Valores permitidos: New (Nuevo), Good (Bueno), Fair (Regular), Poor (Malo).
    /// </remarks>
    /// <example>Good</example>
    public string CondicionGeneral { get; set; } = "Good";

    /// <summary>
    /// Colección de préstamos históricos y activos asociados a este libro
    /// </summary>
    public ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
}