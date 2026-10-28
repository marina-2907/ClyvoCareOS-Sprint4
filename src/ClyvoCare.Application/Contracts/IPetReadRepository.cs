using ClyvoCareOSChallenge.DTOs;

namespace ClyvoCareOSChallenge.Application.Contracts;

public interface IPetReadRepository
{
    Task<(IReadOnlyList<PetResponseDto> Items, int Total)> FindPageAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default);

    Task<PetResponseDto?> FindByIdAsync(
        int id,
        CancellationToken cancellationToken = default);
}
