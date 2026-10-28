using ClyvoCareOSChallenge.Application.Contracts;
using ClyvoCareOSChallenge.DTOs;

namespace ClyvoCareOSChallenge.Application;

public sealed class PetManagementService
{
    private readonly IPetManagementRepository _repository;

    public PetManagementService(IPetManagementRepository repository) => _repository = repository;

    public Task<IReadOnlyList<PetResponseDto>> GetByEspecieAsync(string especie, CancellationToken ct = default) =>
        _repository.FindByEspecieAsync(especie, ct);
    public Task<IReadOnlyList<PetResponseDto>> GetByFaseAsync(string fase, CancellationToken ct = default) =>
        _repository.FindByFaseAsync(fase, ct);
    public Task<IReadOnlyList<PetResponseDto>> GetByNomeAsync(string nome, CancellationToken ct = default) =>
        _repository.FindByNomeAsync(nome, ct);
    public Task<PetHistoricoDto?> GetHistoricoAsync(int id, CancellationToken ct = default) =>
        _repository.FindHistoricoAsync(id, ct);
    public Task<PetResponseDto?> CreateAsync(PetCreateDto dto, CancellationToken ct = default) =>
        _repository.CreateAsync(dto, ct);
    public Task<bool> UpdateAsync(int id, PetUpdateDto dto, CancellationToken ct = default) =>
        _repository.UpdateAsync(id, dto, ct);
    public Task<bool> DeleteAsync(int id, CancellationToken ct = default) =>
        _repository.DeleteAsync(id, ct);
}
