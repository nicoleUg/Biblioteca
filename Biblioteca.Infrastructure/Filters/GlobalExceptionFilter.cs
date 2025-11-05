using Biblioteca.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Infrastructure.Filters;

public class GlobalExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _log;
    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> log) { _log = log; }

    public void OnException(ExceptionContext ctx)
    {
        var ex = ctx.Exception;
        var status = ex is BusinessException be ? be.StatusCode : 500;
        _log.LogError(ex, "Unhandled error");

        ctx.Result = new ObjectResult(new { Message = ex.Message, Type = ex.GetType().Name })
        { StatusCode = status };
        ctx.ExceptionHandled = true;
    }
}
