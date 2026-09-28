using BibliotecaApi.Controllers.Modelos;
using BibliotecaApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaApi.Controllers;

[ApiController]
[Route("api/autores")]
public class AutoresController(IAutorService autorService) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<AutorResponse>> Listar() =>
        (await autorService.ListarAsync()).Select(AutorResponse.De);

    [HttpGet("{id:int}")]
    public async Task<AutorResponse> Obter(int id) =>
        AutorResponse.De(await autorService.ObterAsync(id));

    [HttpPost]
    public async Task<ActionResult<AutorResponse>> Criar(AutorRequest request)
    {
        var autor = await autorService.CriarAsync(request.ParaDados());
        return CreatedAtAction(nameof(Obter), new { id = autor.Id }, AutorResponse.De(autor));
    }

    [HttpPut("{id:int}")]
    public async Task<AutorResponse> Atualizar(int id, AutorRequest request) =>
        AutorResponse.De(await autorService.AtualizarAsync(id, request.ParaDados()));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id)
    {
        await autorService.ExcluirAsync(id);
        return NoContent();
    }
}
