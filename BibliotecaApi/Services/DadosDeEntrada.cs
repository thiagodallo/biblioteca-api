namespace BibliotecaApi.Services;

public record AutorDados(string Nome, string? Nacionalidade, bool Ativo);

public record LivroDados(string Titulo, string Isbn, int AnoPublicacao, int AutorId);
