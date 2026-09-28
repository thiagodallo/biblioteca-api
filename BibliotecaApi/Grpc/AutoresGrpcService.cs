using BibliotecaApi.Domain.Entities;
using BibliotecaApi.Grpc.Contratos;
using BibliotecaApi.Services;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace BibliotecaApi.Grpc;

public class AutoresGrpcService(IAutorService autorService) : Autores.AutoresBase
{
    public override async Task<ListaAutoresReply> Listar(Empty request, ServerCallContext context)
    {
        var reply = new ListaAutoresReply();
        reply.Autores.AddRange((await autorService.ListarAsync()).Select(ParaReply));
        return reply;
    }

    public override async Task<AutorReply> Obter(AutorIdRequest request, ServerCallContext context) =>
        ParaReply(await autorService.ObterAsync(request.Id));

    public override async Task<AutorReply> Criar(AutorDadosRequest request, ServerCallContext context)
    {
        var dados = new AutorDados(request.Nome, request.Nacionalidade, !request.HasAtivo || request.Ativo);
        return ParaReply(await autorService.CriarAsync(dados));
    }

    public override async Task<AutorReply> Atualizar(AtualizarAutorRequest request, ServerCallContext context)
    {
        var dados = new AutorDados(request.Nome, request.Nacionalidade, !request.HasAtivo || request.Ativo);
        return ParaReply(await autorService.AtualizarAsync(request.Id, dados));
    }

    public override async Task<Empty> Excluir(AutorIdRequest request, ServerCallContext context)
    {
        await autorService.ExcluirAsync(request.Id);
        return new Empty();
    }

    private static AutorReply ParaReply(Autor autor) => new()
    {
        Id = autor.Id,
        Nome = autor.Nome,
        Nacionalidade = autor.Nacionalidade ?? string.Empty,
        Ativo = autor.Ativo
    };
}
