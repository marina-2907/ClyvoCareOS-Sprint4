using ClyvoCareOSChallenge.Application;
using ClyvoCareOSChallenge.Application.Contracts;
using ClyvoCareOSChallenge.DTOs;

namespace ClyvoCareOSChallenge.Application;

public sealed class PetQueryService
{
    private readonly IPetReadRepository _repository;

    public PetQueryService(IPetReadRepository repository)
    {
        _repository = repository;
    }

    public Task<(IReadOnlyList<PetResponseDto> Items, int Total)> GetPageAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        return _repository.FindPageAsync(query, cancellationToken);
    }

    public Task<PetResponseDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return _repository.FindByIdAsync(id, cancellationToken);
    }
}

