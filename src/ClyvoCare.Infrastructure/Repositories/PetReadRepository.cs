using ClyvoCareOSChallenge.Application;
using ClyvoCareOSChallenge.Application.Contracts;
using ClyvoCareOSChallenge.Data;
using ClyvoCareOSChallenge.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ClyvoCareOSChallenge.Infrastructure.Repositories;

public sealed class PetReadRepository : IPetReadRepository
{
    private readonly AppDbContext _context;

    public PetReadRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(IReadOnlyList<PetResponseDto> Items, int Total)>
        FindPageAsync(
            PaginationQuery pagination,
            CancellationToken cancellationToken = default)
    {
        var query = _context.Pets.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Nome))
        {
            var nome = pagination.Nome.Trim().ToLower();
            query = query.Where(p =>
                p.Nome.ToLower().Contains(nome));
        }

        if (!string.IsNullOrWhiteSpace(pagination.Especie))
        {
            var especie = pagination.Especie.Trim().ToLower();
            query = query.Where(p =>
                p.Especie.ToLower() == especie);
        }

        var total = await query.CountAsync(cancellationToken);

        var descending = string.Equals(
            pagination.Order,
            "desc",
            StringComparison.OrdinalIgnoreCase);

        query = (pagination.SortBy?.ToLowerInvariant(), descending) switch
        {
            ("nome", false) =>
                query.OrderBy(p => p.Nome).ThenBy(p => p.Id),
            ("nome", true) =>
                query.OrderByDescending(p => p.Nome).ThenBy(p => p.Id),
            ("especie", false) =>
                query.OrderBy(p => p.Especie).ThenBy(p => p.Id),
            ("especie", true) =>
                query.OrderByDescending(p => p.Especie).ThenBy(p => p.Id),
            (_, true) =>
                query.OrderByDescending(p => p.Id),
            _ => query.OrderBy(p => p.Id)
        };

        var items = await query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(p => new PetResponseDto(
                p.Id,
                p.Nome,
                p.Especie,
                p.Raca,
                p.DataNascimento,
                p.Sexo,
                p.PesoKg,
                p.FaseDeVida,
                p.TutorId,
                p.Tutor!.NomeCompleto))
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public Task<PetResponseDto?> FindByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return _context.Pets
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new PetResponseDto(
                p.Id,
                p.Nome,
                p.Especie,
                p.Raca,
                p.DataNascimento,
                p.Sexo,
                p.PesoKg,
                p.FaseDeVida,
                p.TutorId,
                p.Tutor!.NomeCompleto))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
