namespace BibliotecaApi.Domain.Entities;

public class Autor
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Nacionalidade { get; set; }
    public bool Ativo { get; set; } = true;
    public List<Livro> Livros { get; set; } = [];
}
