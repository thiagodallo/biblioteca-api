using BibliotecaApi.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaApi.Controllers;

public class TratadorExcecoesHttp : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not DomainException erro)
            return false;

        var (status, titulo) = erro switch
        {
            RecursoNaoEncontradoException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
            ConflitoException => (StatusCodes.Status409Conflict, "Conflito"),
            RegraNegocioException => (StatusCodes.Status400BadRequest, "Regra de negócio violada"),
            _ => (StatusCodes.Status400BadRequest, "Dados inválidos")
        };

        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(
            new ProblemDetails { Status = status, Title = titulo, Detail = erro.Message },
            options: null,
            contentType: "application/problem+json",
            cancellationToken);
        return true;
    }
}
