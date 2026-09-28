using BibliotecaApi.Domain.Entities;

namespace BibliotecaApi.Repositories;

public interface IAutorRepository
{
    Task<IReadOnlyList<Autor>> ListarAsync();
    Task<Autor?> ObterAsync(int id);
    Task AdicionarAsync(Autor autor);
    Task AtualizarAsync(Autor autor);
    Task RemoverAsync(Autor autor);
}
