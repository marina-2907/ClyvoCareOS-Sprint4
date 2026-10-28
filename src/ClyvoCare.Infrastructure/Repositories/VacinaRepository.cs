using Microsoft.EntityFrameworkCore;
using ClyvoCareOSChallenge.Application.Contracts;
using ClyvoCareOSChallenge.Data;
using ClyvoCareOSChallenge.DTOs;
using ClyvoCareOSChallenge.Models;
namespace ClyvoCareOSChallenge.Infrastructure.Repositories;
public sealed class VacinaRepository : IVacinaRepository {
    private readonly AppDbContext _db;
    public VacinaRepository(AppDbContext db) => _db = db;
    private static VacinaResponseDto Map(Vacina e) => new VacinaResponseDto(e.Id, e.NomeVacina, e.DataAplicacao, e.DataProximaDose, e.Veterinario, e.Clinica, e.Observacoes, e.PetId, e.Pet!.Nome);
    public async Task<IReadOnlyList<VacinaResponseDto>> GetAllAsync() =>
        (await _db.Vacinas.Include(e => e.Pet).AsNoTracking().ToListAsync()).Select(Map).ToList();
    public async Task<VacinaResponseDto?> GetByIdAsync(int id) {
        var e = await _db.Vacinas.Include(e => e.Pet).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return e is null ? null : Map(e);
    }
    public async Task<VacinaResponseDto?> CreateAsync(VacinaCreateDto dto) {
        if (!await _db.Pets.AnyAsync(p => p.Id == dto.PetId)) return null;
        var e = new Vacina { NomeVacina = dto.NomeVacina, DataAplicacao = dto.DataAplicacao, DataProximaDose = dto.DataProximaDose, Veterinario = dto.Veterinario, Clinica = dto.Clinica, Observacoes = dto.Observacoes, PetId = dto.PetId };
        _db.Vacinas.Add(e);
        await _db.SaveChangesAsync();
        await _db.Entry(e).Reference(x => x.Pet).LoadAsync();
        return Map(e);
    }
    public async Task<bool> UpdateAsync(int id, VacinaUpdateDto dto) {
        var e = await _db.Vacinas.FindAsync(id);
        if (e is null) return false;
        e.NomeVacina = dto.NomeVacina;
        e.DataAplicacao = dto.DataAplicacao;
        e.DataProximaDose = dto.DataProximaDose;
        e.Veterinario = dto.Veterinario;
        e.Clinica = dto.Clinica;
        e.Observacoes = dto.Observacoes;
        await _db.SaveChangesAsync();
        return true;
    }
    public async Task<bool> DeleteAsync(int id) {
        var e = await _db.Vacinas.FindAsync(id);
        if (e is null) return false;
        _db.Vacinas.Remove(e);
        await _db.SaveChangesAsync();
        return true;
    }
    public Task<bool> PetExistsAsync(int petId) => _db.Pets.AnyAsync(p => p.Id == petId);
    public async Task<IReadOnlyList<VacinaResponseDto>> FindProximasAsync(int dias) {
        var now = DateTime.UtcNow; var end = now.AddDays(dias);
        var entities = await _db.Vacinas.Include(e => e.Pet).AsNoTracking()
            .Where(e => e.DataProximaDose.HasValue && e.DataProximaDose >= now && e.DataProximaDose <= end).ToListAsync();
        return entities.Select(Map).ToList();
    }
    public async Task<IReadOnlyList<VacinaResponseDto>> FindByPetAsync(int petId) =>
        (await _db.Vacinas.Include(e => e.Pet).AsNoTracking().Where(e => e.PetId == petId).OrderByDescending(e => e.DataAplicacao).ToListAsync()).Select(Map).ToList();
}
