using ClyvoCareOSChallenge.DTOs;
using ClyvoCareOSChallenge.Models;

namespace ClyvoCareOSChallenge.Application.Contracts;

public interface IPetManagementRepository
{
    Task<IReadOnlyList<PetResponseDto>> FindByEspecieAsync(
        string especie, CancellationToken ct = default);

    Task<IReadOnlyList<PetResponseDto>> FindByFaseAsync(
        string fase, CancellationToken ct = default);

    Task<IReadOnlyList<PetResponseDto>> FindByNomeAsync(
        string nome, CancellationToken ct = default);

    Task<PetHistoricoDto?> FindHistoricoAsync(
        int id, CancellationToken ct = default);

    Task<PetResponseDto?> CreateAsync(
        PetCreateDto dto, CancellationToken ct = default);

    Task<bool> UpdateAsync(
        int id, PetUpdateDto dto, CancellationToken ct = default);

    Task<bool> DeleteAsync(
        int id, CancellationToken ct = default);
}

public sealed record PetHistoricoDto(
    PetResponseDto Pet,
    IReadOnlyList<Vacina> Vacinas,
    IReadOnlyList<Consulta> Consultas,
    IReadOnlyList<Tratamento> Tratamentos);
