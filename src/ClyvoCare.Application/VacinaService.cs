using ClyvoCareOSChallenge.Application.Contracts;
using ClyvoCareOSChallenge.DTOs;
namespace ClyvoCareOSChallenge.Application;
public sealed class VacinaService {
    private readonly IVacinaRepository _repo;
    public VacinaService(IVacinaRepository repo) => _repo = repo;
    public Task<IReadOnlyList<VacinaResponseDto>> GetAllAsync() => _repo.GetAllAsync();
    public Task<VacinaResponseDto?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
    public Task<VacinaResponseDto?> CreateAsync(VacinaCreateDto dto) => _repo.CreateAsync(dto);
    public Task<bool> UpdateAsync(int id, VacinaUpdateDto dto) => _repo.UpdateAsync(id, dto);
    public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);
    public Task<bool> PetExistsAsync(int petId) => _repo.PetExistsAsync(petId);
    public Task<IReadOnlyList<VacinaResponseDto>> FindProximasAsync(int dias) => _repo.FindProximasAsync(dias);
    public Task<IReadOnlyList<VacinaResponseDto>> FindByPetAsync(int petId) => _repo.FindByPetAsync(petId);
}
