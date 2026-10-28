using Microsoft.EntityFrameworkCore;
using ClyvoCareOSChallenge.Application.Contracts;
using ClyvoCareOSChallenge.Data;
using ClyvoCareOSChallenge.DTOs;
using ClyvoCareOSChallenge.Models;
namespace ClyvoCareOSChallenge.Infrastructure.Repositories;
public sealed class ConsultaRepository : IConsultaRepository {
    private readonly AppDbContext _db;
    public ConsultaRepository(AppDbContext db) => _db = db;
    private static ConsultaResponseDto Map(Consulta e) => new ConsultaResponseDto(e.Id, e.DataConsulta, e.Motivo, e.Diagnostico, e.Prescricao, e.Veterinario, e.Clinica, e.PesoNaConsulta, e.PetId, e.Pet!.Nome);
    public async Task<IReadOnlyList<ConsultaResponseDto>> GetAllAsync() =>
        (await _db.Consultas.Include(e => e.Pet).AsNoTracking().ToListAsync()).Select(Map).ToList();
    public async Task<ConsultaResponseDto?> GetByIdAsync(int id) {
        var e = await _db.Consultas.Include(e => e.Pet).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return e is null ? null : Map(e);
    }
    public async Task<ConsultaResponseDto?> CreateAsync(ConsultaCreateDto dto) {
        if (!await _db.Pets.AnyAsync(p => p.Id == dto.PetId)) return null;
        var e = new Consulta { DataConsulta = dto.DataConsulta, Motivo = dto.Motivo, Diagnostico = dto.Diagnostico, Prescricao = dto.Prescricao, Veterinario = dto.Veterinario, Clinica = dto.Clinica, PesoNaConsulta = dto.PesoNaConsulta, PetId = dto.PetId };
        _db.Consultas.Add(e);
        await _db.SaveChangesAsync();
        await _db.Entry(e).Reference(x => x.Pet).LoadAsync();
        return Map(e);
    }
    public async Task<bool> UpdateAsync(int id, ConsultaUpdateDto dto) {
        var e = await _db.Consultas.FindAsync(id);
        if (e is null) return false;
        e.DataConsulta = dto.DataConsulta;
        e.Motivo = dto.Motivo;
        e.Diagnostico = dto.Diagnostico;
        e.Prescricao = dto.Prescricao;
        e.Veterinario = dto.Veterinario;
        e.Clinica = dto.Clinica;
        e.PesoNaConsulta = dto.PesoNaConsulta;
        await _db.SaveChangesAsync();
        return true;
    }
    public async Task<bool> DeleteAsync(int id) {
        var e = await _db.Consultas.FindAsync(id);
        if (e is null) return false;
        _db.Consultas.Remove(e);
        await _db.SaveChangesAsync();
        return true;
    }
    public Task<bool> PetExistsAsync(int petId) => _db.Pets.AnyAsync(p => p.Id == petId);
    public async Task<IReadOnlyList<ConsultaResponseDto>> FindByClinicaAsync(string value) =>
        (await _db.Consultas.Include(e => e.Pet).AsNoTracking().Where(e => e.Clinica.ToLower().Contains(value.ToLower())).ToListAsync()).Select(Map).ToList();
    public async Task<IReadOnlyList<ConsultaResponseDto>> FindByPetAsync(int petId) =>
        (await _db.Consultas.Include(e => e.Pet).AsNoTracking().Where(e => e.PetId == petId).OrderByDescending(e => e.DataConsulta).ToListAsync()).Select(Map).ToList();
}
