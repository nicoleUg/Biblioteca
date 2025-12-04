using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Core.CustomEntities;

/// <summary>
/// Representa un Mensaje de respuesta en el sistema
/// </summary>
/// <remarks>
/// Esta entidad almacena la información principal del Mensaje (tipo y descripción) 
/// y es utilizada para estandarizar las respuestas de la API.
/// </remarks>
public class Message
{
    /// <summary>
    /// Tipo de mensaje (categoría)
    /// </summary>
    /// <remarks>
    /// Valores sugeridos: success, warning, error, information.
    /// </remarks>
    /// <example>success</example>
    public string Type { get; set; } = "information";

    /// <summary>
    /// Descripción detallada del mensaje
    /// </summary>
    /// <example>La operación se realizó correctamente.</example>
    public string Description { get; set; } = string.Empty;
}