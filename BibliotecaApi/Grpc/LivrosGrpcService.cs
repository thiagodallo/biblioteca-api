using BibliotecaApi.Domain.Entities;
using BibliotecaApi.Grpc.Contratos;
using BibliotecaApi.Services;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace BibliotecaApi.Grpc;

public class LivrosGrpcService(ILivroService livroService) : Livros.LivrosBase
{
    public override async Task<ListaLivrosReply> Listar(ListarLivrosRequest request, ServerCallContext context)
    {
        int? autorId = request.AutorId > 0 ? request.AutorId : null;

        var reply = new ListaLivrosReply();
        reply.Livros.AddRange((await livroService.ListarAsync(autorId)).Select(ParaReply));
        return reply;
    }

    public override async Task<LivroReply> Obter(LivroIdRequest request, ServerCallContext context) =>
        ParaReply(await livroService.ObterAsync(request.Id));

    public override async Task<LivroReply> Criar(LivroDadosRequest request, ServerCallContext context)
    {
        var dados = new LivroDados(request.Titulo, request.Isbn, request.AnoPublicacao, request.AutorId);
        return ParaReply(await livroService.CriarAsync(dados));
    }

    public override async Task<LivroReply> Atualizar(AtualizarLivroRequest request, ServerCallContext context)
    {
        var dados = new LivroDados(request.Titulo, request.Isbn, request.AnoPublicacao, request.AutorId);
        return ParaReply(await livroService.AtualizarAsync(request.Id, dados));
    }

    public override async Task<Empty> Excluir(LivroIdRequest request, ServerCallContext context)
    {
        await livroService.ExcluirAsync(request.Id);
        return new Empty();
    }

    private static LivroReply ParaReply(Livro livro) => new()
    {
        Id = livro.Id,
        Titulo = livro.Titulo,
        Isbn = livro.Isbn,
        AnoPublicacao = livro.AnoPublicacao,
        AutorId = livro.AutorId,
        AutorNome = livro.Autor?.Nome ?? string.Empty
    };
}
