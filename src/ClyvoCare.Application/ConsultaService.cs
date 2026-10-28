using ClyvoCareOSChallenge.Application.Contracts;
using ClyvoCareOSChallenge.DTOs;
namespace ClyvoCareOSChallenge.Application;
public sealed class ConsultaService {
    private readonly IConsultaRepository _repo;
    public ConsultaService(IConsultaRepository repo) => _repo = repo;
    public Task<IReadOnlyList<ConsultaResponseDto>> GetAllAsync() => _repo.GetAllAsync();
    public Task<ConsultaResponseDto?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
    public Task<ConsultaResponseDto?> CreateAsync(ConsultaCreateDto dto) => _repo.CreateAsync(dto);
    public Task<bool> UpdateAsync(int id, ConsultaUpdateDto dto) => _repo.UpdateAsync(id, dto);
    public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);
    public Task<bool> PetExistsAsync(int petId) => _repo.PetExistsAsync(petId);
    public Task<IReadOnlyList<ConsultaResponseDto>> FindByClinicaAsync(string value) => _repo.FindByClinicaAsync(value);
    public Task<IReadOnlyList<ConsultaResponseDto>> FindByPetAsync(int petId) => _repo.FindByPetAsync(petId);
}
