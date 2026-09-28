using BibliotecaApi.Domain.Entities;

namespace BibliotecaApi.Services;

public interface ILivroService
{
    Task<IReadOnlyList<Livro>> ListarAsync(int? autorId);
    Task<Livro> ObterAsync(int id);
    Task<Livro> CriarAsync(LivroDados dados);
    Task<Livro> AtualizarAsync(int id, LivroDados dados);
    Task ExcluirAsync(int id);
}
