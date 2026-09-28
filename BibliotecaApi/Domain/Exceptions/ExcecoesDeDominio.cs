namespace BibliotecaApi.Domain.Exceptions;

public abstract class DomainException(string message) : Exception(message);

public class RecursoNaoEncontradoException(string message) : DomainException(message);

public class DadosInvalidosException(string message) : DomainException(message);

public class RegraNegocioException(string message) : DomainException(message);

public class ConflitoException(string message) : DomainException(message);
