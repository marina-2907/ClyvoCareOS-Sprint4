using Microsoft.AspNetCore.Mvc;
using ClyvoCareOSChallenge.Application;
using ClyvoCareOSChallenge.DTOs;
namespace ClyvoCareOSChallenge.Controllers;
[ApiController]
[Route("api/tutores")]
[Produces("application/json")]
public sealed class TutoresController : ControllerBase {
    private readonly TutorService _service;
    public TutoresController(TutorService service) => _service = service;
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) {
        var result = await _service.GetByIdAsync(id);
        return result is null ? NotFound(new { mensagem = $"Tutor {id} não encontrado." }) : Ok(result);
    }
    [HttpGet("cidade/{cidade}")]
    public async Task<IActionResult> GetByCidade(string cidade) => Ok(await _service.FindByCidadeAsync(cidade));

    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetByEmail(string email) {
        var result = await _service.FindByEmailAsync(email);
        return result is null ? NotFound(new { mensagem = "Tutor não encontrado." }) : Ok(result);
    }

    [HttpGet("{id:int}/pets")]
    public async Task<IActionResult> GetPetsByTutor(int id) {
        var result = await _service.FindPetsAsync(id);
        return result is null ? NotFound(new { mensagem = $"Tutor {id} não encontrado." }) : Ok(result);
    }
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TutorCreateDto dto) {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _service.CreateAsync(dto);
        if (result is null) return BadRequest(new { mensagem = "E-mail já cadastrado." });
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TutorUpdateDto dto) {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!await _service.UpdateAsync(id, dto)) return NotFound(new { mensagem = $"Tutor {id} não encontrado." });
        return NoContent();
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) {
        if (!await _service.DeleteAsync(id)) return NotFound(new { mensagem = $"Tutor {id} não encontrado." });
        return NoContent();
    }
}
