using ClyvoCareOSChallenge.Application.Contracts;
using ClyvoCareOSChallenge.DTOs;
namespace ClyvoCareOSChallenge.Application;
public sealed class TratamentoService {
    private readonly ITratamentoRepository _repo;
    public TratamentoService(ITratamentoRepository repo) => _repo = repo;
    public Task<IReadOnlyList<TratamentoResponseDto>> GetAllAsync() => _repo.GetAllAsync();
    public Task<TratamentoResponseDto?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
    public Task<TratamentoResponseDto?> CreateAsync(TratamentoCreateDto dto) => _repo.CreateAsync(dto);
    public Task<bool> UpdateAsync(int id, TratamentoUpdateDto dto) => _repo.UpdateAsync(id, dto);
    public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);
    public Task<bool> PetExistsAsync(int petId) => _repo.PetExistsAsync(petId);
    public Task<IReadOnlyList<TratamentoResponseDto>> FindByStatusAsync(string value) => _repo.FindByStatusAsync(value);
    public Task<IReadOnlyList<TratamentoResponseDto>> FindByPetAsync(int petId) => _repo.FindByPetAsync(petId);
}
