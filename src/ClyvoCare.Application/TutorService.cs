using ClyvoCareOSChallenge.Application.Contracts;
using ClyvoCareOSChallenge.DTOs;
namespace ClyvoCareOSChallenge.Application;
public sealed class TutorService {
    private readonly ITutorRepository _repo;
    public TutorService(ITutorRepository repo) => _repo = repo;
    public Task<IReadOnlyList<TutorResponseDto>> GetAllAsync() => _repo.GetAllAsync();
    public Task<TutorResponseDto?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
    public Task<TutorResponseDto?> CreateAsync(TutorCreateDto dto) => _repo.CreateAsync(dto);
    public Task<bool> UpdateAsync(int id, TutorUpdateDto dto) => _repo.UpdateAsync(id, dto);
    public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);
    public Task<IReadOnlyList<TutorResponseDto>> FindByCidadeAsync(string cidade) => _repo.FindByCidadeAsync(cidade);
    public Task<TutorResponseDto?> FindByEmailAsync(string email) => _repo.FindByEmailAsync(email);
    public Task<IReadOnlyList<PetResponseDto>?> FindPetsAsync(int id) => _repo.FindPetsAsync(id);
}
