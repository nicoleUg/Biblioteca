using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Core.Exceptions;

public class BusinessException:Exception
{
    public int StatusCode { get; set; } = 400;
    public string? ErrorCode { get; set; }

    public BusinessException(string message, int statusCode = 400, string? errorCode = null) : base(message)
    { StatusCode = statusCode; ErrorCode = errorCode; }
}
