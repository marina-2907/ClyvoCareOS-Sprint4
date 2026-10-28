using Microsoft.AspNetCore.Mvc;
using ClyvoCareOSChallenge.Application;
using ClyvoCareOSChallenge.DTOs;
namespace ClyvoCareOSChallenge.Controllers;
[ApiController]
[Route("api/consultas")]
[Produces("application/json")]
public sealed class ConsultasController : ControllerBase {
    private readonly ConsultaService _service;
    public ConsultasController(ConsultaService service) => _service = service;
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) {
        var result = await _service.GetByIdAsync(id);
        return result is null ? NotFound(new { mensagem = $"Consulta {id} não encontrado." }) : Ok(result);
    }
    [HttpGet("pet/{petId:int}")]
    public async Task<IActionResult> GetByPet(int petId) {
        if (!await _service.PetExistsAsync(petId)) return NotFound(new { mensagem = $"Pet {petId} não encontrado." });
        return Ok(await _service.FindByPetAsync(petId));
    }
    [HttpGet("clinica/{clinica}")]
    public async Task<IActionResult> GetByClinica(string clinica) => Ok(await _service.FindByClinicaAsync(clinica));
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ConsultaCreateDto dto) {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _service.CreateAsync(dto);
        if (result is null) return BadRequest(new { mensagem = $"Pet {dto.PetId} não encontrado." });
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ConsultaUpdateDto dto) {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!await _service.UpdateAsync(id, dto)) return NotFound(new { mensagem = $"Consulta {id} não encontrado." });
        return NoContent();
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) {
        if (!await _service.DeleteAsync(id)) return NotFound(new { mensagem = $"Consulta {id} não encontrado." });
        return NoContent();
    }
}
