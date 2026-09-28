using System.ComponentModel.DataAnnotations;
using BibliotecaApi.Domain.Entities;
using BibliotecaApi.Services;

namespace BibliotecaApi.Controllers.Modelos;

public record AutorRequest(
    [Required, StringLength(150)] string Nome,
    [StringLength(80)] string? Nacionalidade,
    bool Ativo = true)
{
    public AutorDados ParaDados() => new(Nome, Nacionalidade, Ativo);
}

public record AutorResponse(int Id, string Nome, string? Nacionalidade, bool Ativo)
{
    public static AutorResponse De(Autor autor) =>
        new(autor.Id, autor.Nome, autor.Nacionalidade, autor.Ativo);
}
