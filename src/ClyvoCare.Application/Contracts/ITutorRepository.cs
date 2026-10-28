using ClyvoCareOSChallenge.DTOs;
namespace ClyvoCareOSChallenge.Application.Contracts;
public interface ITutorRepository {
    Task<IReadOnlyList<TutorResponseDto>> GetAllAsync();
    Task<TutorResponseDto?> GetByIdAsync(int id);
    Task<TutorResponseDto?> CreateAsync(TutorCreateDto dto);
    Task<bool> UpdateAsync(int id, TutorUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<IReadOnlyList<TutorResponseDto>> FindByCidadeAsync(string cidade);
    Task<TutorResponseDto?> FindByEmailAsync(string email);
    Task<IReadOnlyList<PetResponseDto>?> FindPetsAsync(int id);
}
