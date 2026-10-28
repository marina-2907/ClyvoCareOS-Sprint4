using Microsoft.EntityFrameworkCore;
using ClyvoCareOSChallenge.Application.Contracts;
using ClyvoCareOSChallenge.Data;
using ClyvoCareOSChallenge.DTOs;
using ClyvoCareOSChallenge.Models;
namespace ClyvoCareOSChallenge.Infrastructure.Repositories;
public sealed class TratamentoRepository : ITratamentoRepository {
    private readonly AppDbContext _db;
    public TratamentoRepository(AppDbContext db) => _db = db;
    private static TratamentoResponseDto Map(Tratamento e) => new TratamentoResponseDto(e.Id, e.NomeTratamento, e.DataInicio, e.DataFim, e.Descricao, e.Status, e.Veterinario, e.PetId, e.Pet!.Nome);
    public async Task<IReadOnlyList<TratamentoResponseDto>> GetAllAsync() =>
        (await _db.Tratamentos.Include(e => e.Pet).AsNoTracking().ToListAsync()).Select(Map).ToList();
    public async Task<TratamentoResponseDto?> GetByIdAsync(int id) {
        var e = await _db.Tratamentos.Include(e => e.Pet).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return e is null ? null : Map(e);
    }
    public async Task<TratamentoResponseDto?> CreateAsync(TratamentoCreateDto dto) {
        if (!await _db.Pets.AnyAsync(p => p.Id == dto.PetId)) return null;
        var e = new Tratamento { NomeTratamento = dto.NomeTratamento, DataInicio = dto.DataInicio, DataFim = dto.DataFim, Descricao = dto.Descricao, Status = dto.Status, Veterinario = dto.Veterinario, PetId = dto.PetId };
        _db.Tratamentos.Add(e);
        await _db.SaveChangesAsync();
        await _db.Entry(e).Reference(x => x.Pet).LoadAsync();
        return Map(e);
    }
    public async Task<bool> UpdateAsync(int id, TratamentoUpdateDto dto) {
        var e = await _db.Tratamentos.FindAsync(id);
        if (e is null) return false;
        e.NomeTratamento = dto.NomeTratamento;
        e.DataInicio = dto.DataInicio;
        e.DataFim = dto.DataFim;
        e.Descricao = dto.Descricao;
        e.Status = dto.Status;
        e.Veterinario = dto.Veterinario;
        await _db.SaveChangesAsync();
        return true;
    }
    public async Task<bool> DeleteAsync(int id) {
        var e = await _db.Tratamentos.FindAsync(id);
        if (e is null) return false;
        _db.Tratamentos.Remove(e);
        await _db.SaveChangesAsync();
        return true;
    }
    public Task<bool> PetExistsAsync(int petId) => _db.Pets.AnyAsync(p => p.Id == petId);
    public async Task<IReadOnlyList<TratamentoResponseDto>> FindByStatusAsync(string value) =>
        (await _db.Tratamentos.Include(e => e.Pet).AsNoTracking().Where(e => e.Status.ToLower() == value.ToLower()).ToListAsync()).Select(Map).ToList();
    public async Task<IReadOnlyList<TratamentoResponseDto>> FindByPetAsync(int petId) =>
        (await _db.Tratamentos.Include(e => e.Pet).AsNoTracking().Where(e => e.PetId == petId).OrderByDescending(e => e.DataInicio).ToListAsync()).Select(Map).ToList();
}
