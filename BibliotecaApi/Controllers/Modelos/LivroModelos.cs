using System.ComponentModel.DataAnnotations;
using BibliotecaApi.Domain.Entities;
using BibliotecaApi.Services;

namespace BibliotecaApi.Controllers.Modelos;

public record LivroRequest(
    [Required, StringLength(200)] string Titulo,
    [Required, StringLength(20)] string Isbn,
    int AnoPublicacao,
    int AutorId)
{
    public LivroDados ParaDados() => new(Titulo, Isbn, AnoPublicacao, AutorId);
}

public record LivroResponse(int Id, string Titulo, string Isbn, int AnoPublicacao, int AutorId, string? AutorNome)
{
    public static LivroResponse De(Livro livro) =>
        new(livro.Id, livro.Titulo, livro.Isbn, livro.AnoPublicacao, livro.AutorId, livro.Autor?.Nome);
}
