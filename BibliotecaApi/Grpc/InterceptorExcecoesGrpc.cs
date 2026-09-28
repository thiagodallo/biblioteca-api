using BibliotecaApi.Domain.Exceptions;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace BibliotecaApi.Grpc;

public class InterceptorExcecoesGrpc : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (DomainException erro)
        {
            var status = erro switch
            {
                RecursoNaoEncontradoException => StatusCode.NotFound,
                ConflitoException => StatusCode.AlreadyExists,
                RegraNegocioException => StatusCode.FailedPrecondition,
                _ => StatusCode.InvalidArgument
            };
            throw new RpcException(new Status(status, erro.Message));
        }
    }
}
