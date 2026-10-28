using ClyvoCareOSChallenge.DTOs;
namespace ClyvoCareOSChallenge.Application.Contracts;
public interface ITratamentoRepository {
    Task<IReadOnlyList<TratamentoResponseDto>> GetAllAsync();
    Task<TratamentoResponseDto?> GetByIdAsync(int id);
    Task<TratamentoResponseDto?> CreateAsync(TratamentoCreateDto dto);
    Task<bool> UpdateAsync(int id, TratamentoUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> PetExistsAsync(int petId);
    Task<IReadOnlyList<TratamentoResponseDto>> FindByStatusAsync(string value);
    Task<IReadOnlyList<TratamentoResponseDto>> FindByPetAsync(int petId);
}
