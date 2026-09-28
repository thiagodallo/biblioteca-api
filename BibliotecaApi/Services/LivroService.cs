using BibliotecaApi.Domain.Entities;
using BibliotecaApi.Domain.Exceptions;
using BibliotecaApi.Repositories;

namespace BibliotecaApi.Services;

public class LivroService(ILivroRepository livros, IAutorRepository autores) : ILivroService
{
    public Task<IReadOnlyList<Livro>> ListarAsync(int? autorId) => livros.ListarAsync(autorId);

    public async Task<Livro> ObterAsync(int id) =>
        await livros.ObterAsync(id)
        ?? throw new RecursoNaoEncontradoException($"Livro {id} não encontrado.");

    public async Task<Livro> CriarAsync(LivroDados dados)
    {
        ValidarCampos(dados);
        var autor = await ObterAutorAtivoAsync(dados.AutorId);

        if (await livros.IsbnEmUsoAsync(dados.Isbn))
            throw new ConflitoException($"Já existe um livro com o ISBN {dados.Isbn}.");

        var livro = new Livro();
        Preencher(livro, dados, autor);
        await livros.AdicionarAsync(livro);
        return livro;
    }

    public async Task<Livro> AtualizarAsync(int id, LivroDados dados)
    {
        ValidarCampos(dados);
        var livro = await ObterAsync(id);
        var autor = await ObterAutorAtivoAsync(dados.AutorId);

        if (await livros.IsbnEmUsoAsync(dados.Isbn, ignorarLivroId: id))
            throw new ConflitoException($"Já existe outro livro com o ISBN {dados.Isbn}.");

        Preencher(livro, dados, autor);
        await livros.AtualizarAsync(livro);
        return livro;
    }

    public async Task ExcluirAsync(int id)
    {
        var livro = await ObterAsync(id);
        await livros.RemoverAsync(livro);
    }

    private async Task<Autor> ObterAutorAtivoAsync(int autorId)
    {
        var autor = await autores.ObterAsync(autorId)
            ?? throw new RecursoNaoEncontradoException($"Autor {autorId} não encontrado.");

        if (!autor.Ativo)
            throw new RegraNegocioException($"O autor '{autor.Nome}' está inativo e não pode receber livros.");

        return autor;
    }

    private static void ValidarCampos(LivroDados dados)
    {
        if (string.IsNullOrWhiteSpace(dados.Titulo))
            throw new DadosInvalidosException("O título do livro é obrigatório.");

        if (string.IsNullOrWhiteSpace(dados.Isbn))
            throw new DadosInvalidosException("O ISBN do livro é obrigatório.");

        if (dados.AnoPublicacao <= 0 || dados.AnoPublicacao > DateTime.UtcNow.Year)
            throw new DadosInvalidosException($"Ano de publicação inválido: {dados.AnoPublicacao}.");
    }

    private static void Preencher(Livro livro, LivroDados dados, Autor autor)
    {
        livro.Titulo = dados.Titulo.Trim();
        livro.Isbn = dados.Isbn.Trim();
        livro.AnoPublicacao = dados.AnoPublicacao;
        livro.AutorId = autor.Id;
        livro.Autor = autor;
    }
}
