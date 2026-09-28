using BibliotecaApi.Domain.Entities;

namespace BibliotecaApi.Services;

public interface IAutorService
{
    Task<IReadOnlyList<Autor>> ListarAsync();
    Task<Autor> ObterAsync(int id);
    Task<Autor> CriarAsync(AutorDados dados);
    Task<Autor> AtualizarAsync(int id, AutorDados dados);
    Task ExcluirAsync(int id);
}
