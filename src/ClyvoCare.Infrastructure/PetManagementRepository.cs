using ClyvoCareOSChallenge.Application.Contracts;
using ClyvoCareOSChallenge.Data;
using ClyvoCareOSChallenge.DTOs;
using ClyvoCareOSChallenge.Models;
using Microsoft.EntityFrameworkCore;

namespace ClyvoCareOSChallenge.Infrastructure.Repositories;

public sealed class PetManagementRepository : IPetManagementRepository
{
    private readonly AppDbContext _context;
    public PetManagementRepository(AppDbContext context) => _context = context;

    private static PetResponseDto Map(Pet p) => new(
        p.Id, p.Nome, p.Especie, p.Raca, p.DataNascimento,
        p.Sexo, p.PesoKg, p.FaseDeVida, p.TutorId,
        p.Tutor?.NomeCompleto ?? string.Empty);

    public async Task<IReadOnlyList<PetResponseDto>> FindByEspecieAsync(string especie, CancellationToken ct = default)
    {
        var filtro = especie.Trim().ToLower();
        var pets = await _context.Pets.AsNoTracking().Include(p => p.Tutor)
            .Where(p => p.Especie.ToLower() == filtro).ToListAsync(ct);
        return pets.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<PetResponseDto>> FindByFaseAsync(string fase, CancellationToken ct = default)
    {
        // FaseDeVida é uma propriedade [NotMapped], portanto o filtro é realizado em memória.
        var pets = await _context.Pets.AsNoTracking().Include(p => p.Tutor).ToListAsync(ct);
        return pets.Where(p => string.Equals(p.FaseDeVida, fase.Trim(), StringComparison.OrdinalIgnoreCase))
            .Select(Map).ToList();
    }

    public async Task<IReadOnlyList<PetResponseDto>> FindByNomeAsync(string nome, CancellationToken ct = default)
    {
        var filtro = nome.Trim().ToLower();
        var pets = await _context.Pets.AsNoTracking().Include(p => p.Tutor)
            .Where(p => p.Nome.ToLower().Contains(filtro)).ToListAsync(ct);
        return pets.Select(Map).ToList();
    }

    public async Task<PetHistoricoDto?> FindHistoricoAsync(int id, CancellationToken ct = default)
    {
        var pet = await _context.Pets.AsNoTracking().Include(p => p.Tutor)
            .Include(p => p.Vacinas).Include(p => p.Consultas).Include(p => p.Tratamentos)
            .AsSplitQuery().FirstOrDefaultAsync(p => p.Id == id, ct);
        if (pet is null) return null;
        return new PetHistoricoDto(Map(pet),
            pet.Vacinas.OrderByDescending(v => v.DataAplicacao).ToList(),
            pet.Consultas.OrderByDescending(c => c.DataConsulta).ToList(),
            pet.Tratamentos.OrderByDescending(t => t.DataInicio).ToList());
    }

    public async Task<PetResponseDto?> CreateAsync(PetCreateDto dto, CancellationToken ct = default)
    {
        if (!await _context.Tutores.AnyAsync(t => t.Id == dto.TutorId, ct)) return null;
        var pet = new Pet
        {
            Nome = dto.Nome, Especie = dto.Especie, Raca = dto.Raca,
            DataNascimento = dto.DataNascimento, Sexo = dto.Sexo,
            PesoKg = dto.PesoKg, TutorId = dto.TutorId
        };
        _context.Pets.Add(pet);
        await _context.SaveChangesAsync(ct);
        await _context.Entry(pet).Reference(p => p.Tutor).LoadAsync(ct);
        return Map(pet);
    }

    public async Task<bool> UpdateAsync(int id, PetUpdateDto dto, CancellationToken ct = default)
    {
        var pet = await _context.Pets.FindAsync(new object[] { id }, ct);
        if (pet is null) return false;
        pet.Nome = dto.Nome;
        pet.Especie = dto.Especie;
        pet.Raca = dto.Raca;
        pet.DataNascimento = dto.DataNascimento;
        pet.Sexo = dto.Sexo;
        pet.PesoKg = dto.PesoKg;
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var pet = await _context.Pets.FindAsync(new object[] { id }, ct);
        if (pet is null) return false;
        _context.Pets.Remove(pet);
        await _context.SaveChangesAsync(ct);
        return true;
    }
}
