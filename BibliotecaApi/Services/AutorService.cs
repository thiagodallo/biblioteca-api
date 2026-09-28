using BibliotecaApi.Domain.Entities;
using BibliotecaApi.Domain.Exceptions;
using BibliotecaApi.Repositories;

namespace BibliotecaApi.Services;

public class AutorService(IAutorRepository autores, ILivroRepository livros) : IAutorService
{
    public Task<IReadOnlyList<Autor>> ListarAsync() => autores.ListarAsync();

    public async Task<Autor> ObterAsync(int id) =>
        await autores.ObterAsync(id)
        ?? throw new RecursoNaoEncontradoException($"Autor {id} não encontrado.");

    public async Task<Autor> CriarAsync(AutorDados dados)
    {
        ValidarNome(dados.Nome);

        var autor = new Autor();
        Preencher(autor, dados);
        await autores.AdicionarAsync(autor);
        return autor;
    }

    public async Task<Autor> AtualizarAsync(int id, AutorDados dados)
    {
        ValidarNome(dados.Nome);

        var autor = await ObterAsync(id);
        Preencher(autor, dados);
        await autores.AtualizarAsync(autor);
        return autor;
    }

    public async Task ExcluirAsync(int id)
    {
        var autor = await ObterAsync(id);
        if (await livros.AutorPossuiLivrosAsync(id))
            throw new RegraNegocioException(
                $"O autor '{autor.Nome}' possui livros cadastrados. Exclua os livros ou apenas desative o autor.");

        await autores.RemoverAsync(autor);
    }

    private static void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DadosInvalidosException("O nome do autor é obrigatório.");
    }

    private static void Preencher(Autor autor, AutorDados dados)
    {
        autor.Nome = dados.Nome.Trim();
        autor.Nacionalidade = string.IsNullOrWhiteSpace(dados.Nacionalidade) ? null : dados.Nacionalidade.Trim();
        autor.Ativo = dados.Ativo;
    }
}
