using BibliotecaApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApi.Repositories;

public static class SeedInicial
{
    public static async Task PrepararBancoAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BibliotecaContext>();

        await db.Database.EnsureCreatedAsync();
        if (await db.Autores.AnyAsync())
            return;

        db.Autores.Add(new Autor
        {
            Nome = "Machado de Assis",
            Nacionalidade = "Brasileira",
            Livros =
            [
                new Livro { Titulo = "Dom Casmurro", Isbn = "9788535910667", AnoPublicacao = 1899 },
                new Livro { Titulo = "Memórias Póstumas de Brás Cubas", Isbn = "9788535911350", AnoPublicacao = 1881 }
            ]
        });
        await db.SaveChangesAsync();

        db.Autores.Add(new Autor
        {
            Nome = "Clarice Lispector",
            Nacionalidade = "Brasileira",
            Livros = [new Livro { Titulo = "A Hora da Estrela", Isbn = "9788532508126", AnoPublicacao = 1977 }]
        });
        await db.SaveChangesAsync();

        db.Autores.Add(new Autor { Nome = "George Orwell", Nacionalidade = "Britânica", Ativo = false });
        await db.SaveChangesAsync();
    }
}
