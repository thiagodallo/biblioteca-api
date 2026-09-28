using BibliotecaApi.Controllers.Modelos;
using BibliotecaApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaApi.Controllers;

[ApiController]
[Route("api/livros")]
public class LivrosController(ILivroService livroService) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<LivroResponse>> Listar([FromQuery] int? autorId) =>
        (await livroService.ListarAsync(autorId)).Select(LivroResponse.De);

    [HttpGet("{id:int}")]
    public async Task<LivroResponse> Obter(int id) =>
        LivroResponse.De(await livroService.ObterAsync(id));

    [HttpPost]
    public async Task<ActionResult<LivroResponse>> Criar(LivroRequest request)
    {
        var livro = await livroService.CriarAsync(request.ParaDados());
        return CreatedAtAction(nameof(Obter), new { id = livro.Id }, LivroResponse.De(livro));
    }

    [HttpPut("{id:int}")]
    public async Task<LivroResponse> Atualizar(int id, LivroRequest request) =>
        LivroResponse.De(await livroService.AtualizarAsync(id, request.ParaDados()));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id)
    {
        await livroService.ExcluirAsync(id);
        return NoContent();
    }
}
