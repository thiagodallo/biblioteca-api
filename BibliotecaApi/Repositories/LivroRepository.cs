using BibliotecaApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApi.Repositories;

public class LivroRepository(BibliotecaContext db) : ILivroRepository
{
    public async Task<IReadOnlyList<Livro>> ListarAsync(int? autorId)
    {
        var consulta = db.Livros.AsNoTracking().Include(l => l.Autor).AsQueryable();
        if (autorId is not null)
            consulta = consulta.Where(l => l.AutorId == autorId);

        return await consulta.OrderBy(l => l.Titulo).ToListAsync();
    }

    public Task<Livro?> ObterAsync(int id) =>
        db.Livros.Include(l => l.Autor).FirstOrDefaultAsync(l => l.Id == id);

    public Task<bool> IsbnEmUsoAsync(string isbn, int? ignorarLivroId = null) =>
        db.Livros.AnyAsync(l => l.Isbn == isbn && l.Id != ignorarLivroId);

    public Task<bool> AutorPossuiLivrosAsync(int autorId) =>
        db.Livros.AnyAsync(l => l.AutorId == autorId);

    public async Task AdicionarAsync(Livro livro)
    {
        db.Livros.Add(livro);
        await db.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Livro livro)
    {
        db.Livros.Update(livro);
        await db.SaveChangesAsync();
    }

    public async Task RemoverAsync(Livro livro)
    {
        db.Livros.Remove(livro);
        await db.SaveChangesAsync();
    }
}
