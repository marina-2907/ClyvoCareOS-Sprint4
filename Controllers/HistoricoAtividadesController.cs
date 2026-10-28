using ClyvoCareOSChallenge.Application.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ClyvoCareOSChallenge.Controllers;

public sealed record CriarAtividadeDto(
    [Required, Range(1, int.MaxValue)] int PetId,
    [Required, StringLength(50)] string Tipo,
    [Required, StringLength(500)] string Descricao
);

[ApiController]
[Route("api/historico-atividades")]
[Authorize(AuthenticationSchemes =
    JwtBearerDefaults.AuthenticationScheme)]
public class HistoricoAtividadesController : ControllerBase
{
    private readonly IHistoricoRepository _repository;

    public HistoricoAtividadesController(
        IHistoricoRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<IActionResult> Criar(
        [FromBody] CriarAtividadeDto dto)
    {
        AtividadeHistoricoDto atividade = await _repository.CriarAsync(dto.PetId, dto.Tipo, dto.Descricao);

        return CreatedAtAction(
            nameof(ListarPorPet),
            new { petId = atividade.PetId },
            atividade);
    }

    [HttpGet("pet/{petId:int}")]
    public async Task<IActionResult> ListarPorPet(int petId)
    {
        if (petId <= 0)
            return BadRequest(new { mensagem = "PetId inválido." });

        IReadOnlyList<AtividadeHistoricoDto> atividades =
            await _repository.ListarPorPetAsync(petId);

        return Ok(atividades);
    }
}
