using BibliotecaApi.Domain.Entities;

namespace BibliotecaApi.Repositories;

public interface ILivroRepository
{
    Task<IReadOnlyList<Livro>> ListarAsync(int? autorId);
    Task<Livro?> ObterAsync(int id);
    Task<bool> IsbnEmUsoAsync(string isbn, int? ignorarLivroId = null);
    Task<bool> AutorPossuiLivrosAsync(int autorId);
    Task AdicionarAsync(Livro livro);
    Task AtualizarAsync(Livro livro);
    Task RemoverAsync(Livro livro);
}
