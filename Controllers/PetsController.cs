using ClyvoCareOSChallenge.Application;
using ClyvoCareOSChallenge.Application.Contracts;
using ClyvoCareOSChallenge.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ClyvoCareOSChallenge.Controllers;

[ApiController]
[Route("api/pets")]
[Produces("application/json")]
public class PetsController : ControllerBase
{
    private readonly PetQueryService _queries;
    private readonly PetManagementService _management;

    public PetsController(PetQueryService queries, PetManagementService management)
    {
        _queries = queries;
        _management = management;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaginationQuery pagination, CancellationToken ct)
    {
        (IReadOnlyList<PetResponseDto>? items, int totalItems) = await _queries.GetPageAsync(pagination, ct);
        var pages = (int)Math.Ceiling((double)totalItems / pagination.PageSize);

        string LinkTo(int page) => Url.ActionLink(nameof(GetAll), values: new
        {
            page,
            pageSize = pagination.PageSize,
            nome = pagination.Nome,
            especie = pagination.Especie,
            sortBy = pagination.SortBy,
            order = pagination.Order
        }) ?? $"/api/pets?page={page}&pageSize={pagination.PageSize}";

        var links = new Dictionary<string, string> { ["self"] = LinkTo(pagination.Page) };
        if (pagination.Page > 1) links["prev"] = LinkTo(pagination.Page - 1);
        if (pagination.Page < pages) links["next"] = LinkTo(pagination.Page + 1);

        return Ok(new PagedResponse<PetResponseDto>(items, pagination.Page,
            pagination.PageSize, totalItems, pages, links));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        PetResponseDto? pet = await _queries.GetByIdAsync(id, ct);
        return pet is null ? NotFound(new { mensagem = $"Pet {id} não encontrado." }) : Ok(pet);
    }

    [HttpGet("especie/{especie}")]
    public async Task<IActionResult> GetByEspecie(string especie, CancellationToken ct) =>
        Ok(await _management.GetByEspecieAsync(especie, ct));

    [HttpGet("fase/{fase}")]
    public async Task<IActionResult> GetByFase(string fase, CancellationToken ct) =>
        Ok(await _management.GetByFaseAsync(fase, ct));

    [HttpGet("buscar/{nome}")]
    public async Task<IActionResult> GetByNome(string nome, CancellationToken ct) =>
        Ok(await _management.GetByNomeAsync(nome, ct));

    [HttpGet("{id:int}/historico")]
    public async Task<IActionResult> GetHistorico(int id, CancellationToken ct)
    {
        PetHistoricoDto? historico = await _management.GetHistoricoAsync(id, ct);
        return historico is null
            ? NotFound(new { mensagem = $"Pet {id} não encontrado." })
            : Ok(new { pet = historico.Pet, vacinas = historico.Vacinas,
                consultas = historico.Consultas, tratamentos = historico.Tratamentos });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PetCreateDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        PetResponseDto? pet = await _management.CreateAsync(dto, ct);
        return pet is null
            ? BadRequest(new { mensagem = $"Tutor {dto.TutorId} não encontrado." })
            : CreatedAtAction(nameof(GetById), new { id = pet.Id }, pet);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] PetUpdateDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var atualizado = await _management.UpdateAsync(id, dto, ct);
        return atualizado ? NoContent() : NotFound(new { mensagem = $"Pet {id} não encontrado." });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var excluido = await _management.DeleteAsync(id, ct);
        return excluido ? NoContent() : NotFound(new { mensagem = $"Pet {id} não encontrado." });
    }
}
