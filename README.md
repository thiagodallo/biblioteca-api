# Biblioteca API

Trabalho de Arquitetura de Software: uma API de biblioteca que expõe os mesmos casos de uso por **REST** e por **gRPC**, organizada em camadas **Apresentação → Domínio → Repositório**.

**Grupo:** Thiago, Murilo, Cauã e Vinicius

## Domínio

Duas entidades relacionadas:

- **Autor**: nome, nacionalidade e se está ativo.
- **Livro**: título, ISBN, ano de publicação e o autor (N livros para 1 autor).

Regras de negócio que cruzam os dois agregados, todas implementadas na camada de domínio (`Services/`):

| Regra | Onde | Exceção de domínio |
|---|---|---|
| Um livro só pode ser cadastrado/atualizado para um autor **ativo** | `LivroService.ObterAutorAtivoAsync` | `RegraNegocioException` |
| O autor informado no livro precisa existir | `LivroService.ObterAutorAtivoAsync` | `RecursoNaoEncontradoException` |
| Um autor com livros cadastrados não pode ser excluído (só desativado) | `AutorService.ExcluirAsync` | `RegraNegocioException` |
| O ISBN é único | `LivroService.CriarAsync` / `AtualizarAsync` | `ConflitoException` |

## Como rodar

Pré-requisito: Docker Desktop.

```bash
docker compose up --build
```

Isso sobe o PostgreSQL e a API. Na primeira execução o banco é criado e populado com 3 autores e 3 livros. O autor **3 (George Orwell) começa inativo** para facilitar o teste da regra.

Para zerar o banco e voltar aos dados iniciais:

```bash
docker compose down -v
docker compose up --build
```

### Portas

| Porta | Protocolo | Uso |
|---|---|---|
| `8080` | HTTP/1.1 | REST (`/api/autores`, `/api/livros`) |
| `8081` | HTTP/2 (sem TLS) | gRPC (`biblioteca.Autores`, `biblioteca.Livros`) |
| `5433` | TCP | PostgreSQL, caso queira abrir o banco no DBeaver/pgAdmin (usuário e senha `biblioteca`) |

Elas ficam em portas separadas porque o gRPC exige HTTP/2 e o Postman/curl falam HTTP/1.1 no REST; sem TLS, o Kestrel não negocia os dois protocolos na mesma porta.

## Onde cada camada vive

```
BibliotecaApi/
├── Controllers/          Apresentação REST
│   ├── AutoresController.cs, LivrosController.cs
│   ├── Modelos/          DTOs de request/response com validação de formato ([Required], [StringLength])
│   └── TratadorExcecoesHttp.cs   exceção de domínio → status HTTP
├── Grpc/                 Apresentação gRPC
│   ├── AutoresGrpcService.cs, LivrosGrpcService.cs
│   └── InterceptorExcecoesGrpc.cs  exceção de domínio → status gRPC
├── Protos/
│   └── biblioteca.proto  contrato gRPC (contract-first, as classes são geradas no build)
├── Services/             Domínio: IAutorService, ILivroService e as regras de negócio
├── Domain/
│   ├── Entities/         Autor, Livro
│   └── Exceptions/       exceções de domínio
├── Repositories/         Repositório: interfaces, implementações com EF Core e o DbContext
└── Program.cs            composição (injeção de dependência e endpoints)
```

Os pontos principais da arquitetura:

- `AutoresController` e `AutoresGrpcService` recebem a **mesma** interface `IAutorService` por injeção de dependência (o mesmo vale para livros). A regra existe em um lugar só, e os dois lados de apresentação só traduzem o formato de entrada/saída.
- Nenhuma classe de `Controllers/` ou `Grpc/` conhece o `BibliotecaContext` ou o EF Core. Só `Repositories/` fala com o banco.
- Os Controllers e GrpcServices não têm nenhum `if` decidindo status. O domínio lança uma exceção, e cada lado da apresentação traduz ela num lugar só:

| Exceção de domínio | HTTP | gRPC |
|---|---|---|
| `RecursoNaoEncontradoException` | 404 Not Found | `NotFound` |
| `ConflitoException` | 409 Conflict | `AlreadyExists` |
| `RegraNegocioException` | 400 Bad Request | `FailedPrecondition` |
| `DadosInvalidosException` | 400 Bad Request | `InvalidArgument` |

## Exemplos REST (curl)

```bash
# listar autores
curl localhost:8080/api/autores

# criar autor
curl -X POST localhost:8080/api/autores -H "Content-Type: application/json" \
  -d '{"nome":"Lygia Fagundes Telles","nacionalidade":"Brasileira","ativo":true}'

# listar livros (todos ou de um autor)
curl localhost:8080/api/livros
curl "localhost:8080/api/livros?autorId=1"

# criar livro -> 201
curl -X POST localhost:8080/api/livros -H "Content-Type: application/json" \
  -d '{"titulo":"Quincas Borba","isbn":"9788508140028","anoPublicacao":1891,"autorId":1}'

# atualizar livro
curl -X PUT localhost:8080/api/livros/1 -H "Content-Type: application/json" \
  -d '{"titulo":"Dom Casmurro (edição comentada)","isbn":"9788535910667","anoPublicacao":1899,"autorId":1}'

# excluir livro -> 204
curl -X DELETE localhost:8080/api/livros/3
```

Casos de erro:

```bash
# autor inativo -> 400 Regra de negócio violada
curl -X POST localhost:8080/api/livros -H "Content-Type: application/json" \
  -d '{"titulo":"1984","isbn":"9788535914849","anoPublicacao":1949,"autorId":3}'

# ISBN repetido -> 409 Conflito
curl -X POST localhost:8080/api/livros -H "Content-Type: application/json" \
  -d '{"titulo":"Outro","isbn":"9788535910667","anoPublicacao":1900,"autorId":1}'

# autor inexistente -> 404
curl -X POST localhost:8080/api/livros -H "Content-Type: application/json" \
  -d '{"titulo":"Outro","isbn":"123","anoPublicacao":1900,"autorId":99}'

# excluir autor que tem livros -> 400
curl -X DELETE localhost:8080/api/autores/1
```

Exemplo de resposta de erro:

```json
{
  "title": "Regra de negócio violada",
  "status": 400,
  "detail": "O autor 'George Orwell' está inativo e não pode receber livros."
}
```

> No Windows, o curl do Git Bash às vezes envia acentos fora de UTF-8 e a API responde 400 de JSON inválido. Pelo Postman isso não acontece.

## Exemplos gRPC (grpcurl)

A API tem **server reflection** ligado, então o grpcurl e o Postman descobrem os serviços sem precisar do `.proto`.

```bash
grpcurl -plaintext localhost:8081 list
grpcurl -plaintext localhost:8081 describe biblioteca.Livros

grpcurl -plaintext localhost:8081 biblioteca.Autores/Listar
grpcurl -plaintext -d '{"id":1}' localhost:8081 biblioteca.Autores/Obter
grpcurl -plaintext -d '{"nome":"Cecília Meireles","nacionalidade":"Brasileira"}' localhost:8081 biblioteca.Autores/Criar

grpcurl -plaintext -d '{"autor_id":1}' localhost:8081 biblioteca.Livros/Listar
grpcurl -plaintext -d '{"titulo":"Helena","isbn":"9788508047471","ano_publicacao":1876,"autor_id":1}' \
  localhost:8081 biblioteca.Livros/Criar
grpcurl -plaintext -d '{"id":1}' localhost:8081 biblioteca.Livros/Excluir

# autor inativo -> Code: FailedPrecondition
grpcurl -plaintext -d '{"titulo":"1984","isbn":"9788535914849","ano_publicacao":1949,"autor_id":3}' \
  localhost:8081 biblioteca.Livros/Criar
```

Sem o grpcurl instalado, dá para rodar pela imagem Docker (troque `localhost` por `host.docker.internal`):

```bash
docker run --rm fullstorydev/grpcurl -plaintext host.docker.internal:8081 biblioteca.Autores/Listar
```

Observações do contrato:

- Em `Autores/Criar` e `Autores/Atualizar`, o campo `ativo` é `optional`: se não for enviado, o autor fica ativo.
- Em `Livros/Listar`, `autor_id = 0` (ou omitido) lista todos os livros.
- O proto3 omite campos com valor padrão na resposta (ex.: `"ativo": false` não aparece). Use `-emit-defaults` no grpcurl para mostrar.

## Testando pelo Postman

**REST:** importe `postman/Biblioteca-REST.postman_collection.json`. Ela tem o CRUD de autores e livros e uma pasta com os casos de erro.

**gRPC:**

1. **New → gRPC**.
2. URL: `localhost:8081` (deixe o cadeado do TLS desligado).
3. Em **Select a method**, escolha **Use server reflection**. Os serviços `biblioteca.Autores` e `biblioteca.Livros` aparecem na lista. Se preferir, dá para importar `BibliotecaApi/Protos/biblioteca.proto`.
4. Escolha o método, clique em **Use Example Message** para gerar o JSON, ajuste e clique em **Invoke**.

## Rodando fora do Docker (opcional)

Com o .NET 8 SDK instalado e o banco do compose de pé (`docker compose up -d db`):

```bash
cd BibliotecaApi
dotnet run
```

A connection string em `appsettings.json` já aponta para `localhost:5433`.

## Tecnologias

ASP.NET Core 8, Grpc.AspNetCore, Entity Framework Core com Npgsql, PostgreSQL 16 e Docker Compose.
