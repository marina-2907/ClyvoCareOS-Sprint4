using ClyvoCareOSChallenge.DTOs;
namespace ClyvoCareOSChallenge.Application.Contracts;
public interface IConsultaRepository {
    Task<IReadOnlyList<ConsultaResponseDto>> GetAllAsync();
    Task<ConsultaResponseDto?> GetByIdAsync(int id);
    Task<ConsultaResponseDto?> CreateAsync(ConsultaCreateDto dto);
    Task<bool> UpdateAsync(int id, ConsultaUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> PetExistsAsync(int petId);
    Task<IReadOnlyList<ConsultaResponseDto>> FindByClinicaAsync(string value);
    Task<IReadOnlyList<ConsultaResponseDto>> FindByPetAsync(int petId);
}
