using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Core.DTOs;

/// <summary>
/// Representa un Libro (DTO) en el sistema
/// </summary>
/// <remarks>
/// Este objeto se utiliza para transferir la información de los libros 
/// en las respuestas de la API hacia el cliente.
/// </remarks>
public class LibroDto
{
    /// <summary>
    /// Identificador único del libro
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>
    /// Título de la obra
    /// </summary>
    /// <example>El Principito</example>
    public string Titulo { get; set; } = "";

    /// <summary>
    /// Nombre del autor o autores
    /// </summary>
    /// <example>Antoine de Saint-Exupéry</example>
    public string Autor { get; set; } = "";

    /// <summary>
    /// Categoría o género literario
    /// </summary>
    /// <example>Fábula</example>
    public string Categoria { get; set; } = "General";

    /// <summary>
    /// Costo de reposición en Bolivianos en caso de pérdida
    /// </summary>
    /// <example>85.50</example>
    public decimal CostoReposicionBs { get; set; }

    /// <summary>
    /// Cantidad total de ejemplares físicos en inventario
    /// </summary>
    /// <example>10</example>
    public int TotalEjemplares { get; set; }

    /// <summary>
    /// Cantidad de ejemplares disponibles para préstamo
    /// </summary>
    /// <example>8</example>
    public int EjemplaresDisponibles { get; set; }

    /// <summary>
    /// Condición física general de los ejemplares
    /// </summary>
    /// <remarks>
    /// Valores posibles: New, Good, Fair, Poor.
    /// </remarks>
    /// <example>Good</example>
    public string CondicionGeneral { get; set; } = "Good";

    /// <summary>
    /// Indica si el libro está habilitado en el catálogo
    /// </summary>
    /// <example>true</example>
    public bool Habilitado { get; set; } = true;
}