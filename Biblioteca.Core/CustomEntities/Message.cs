using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Core.CustomEntities;

public class Message
{
    public string Type { get; set; } = "information"; // success|warning|error|information
    public string Description { get; set; } = "";
}
