using BibliotecaApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApi.Repositories;

public class AutorRepository(BibliotecaContext db) : IAutorRepository
{
    public async Task<IReadOnlyList<Autor>> ListarAsync() =>
        await db.Autores.AsNoTracking().OrderBy(a => a.Nome).ToListAsync();

    public Task<Autor?> ObterAsync(int id) =>
        db.Autores.FirstOrDefaultAsync(a => a.Id == id);

    public async Task AdicionarAsync(Autor autor)
    {
        db.Autores.Add(autor);
        await db.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Autor autor)
    {
        db.Autores.Update(autor);
        await db.SaveChangesAsync();
    }

    public async Task RemoverAsync(Autor autor)
    {
        db.Autores.Remove(autor);
        await db.SaveChangesAsync();
    }
}
