using ClyvoCareOSChallenge.DTOs;
namespace ClyvoCareOSChallenge.Application.Contracts;
public interface IVacinaRepository {
    Task<IReadOnlyList<VacinaResponseDto>> GetAllAsync();
    Task<VacinaResponseDto?> GetByIdAsync(int id);
    Task<VacinaResponseDto?> CreateAsync(VacinaCreateDto dto);
    Task<bool> UpdateAsync(int id, VacinaUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> PetExistsAsync(int petId);
    Task<IReadOnlyList<VacinaResponseDto>> FindProximasAsync(int dias);
    Task<IReadOnlyList<VacinaResponseDto>> FindByPetAsync(int petId);
}
