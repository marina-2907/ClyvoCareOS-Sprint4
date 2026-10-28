using Microsoft.EntityFrameworkCore;
using ClyvoCareOSChallenge.Application.Contracts;
using ClyvoCareOSChallenge.Data;
using ClyvoCareOSChallenge.DTOs;
using ClyvoCareOSChallenge.Models;
namespace ClyvoCareOSChallenge.Infrastructure.Repositories;
public sealed class TutorRepository : ITutorRepository {
    private readonly AppDbContext _db;
    public TutorRepository(AppDbContext db) => _db = db;
    private static TutorResponseDto Map(Tutor e) => new TutorResponseDto(e.Id, e.NomeCompleto, e.Email, e.Telefone, e.Cidade, e.Estado, e.DataCadastro);
    public async Task<IReadOnlyList<TutorResponseDto>> GetAllAsync() =>
        (await _db.Tutores.AsNoTracking().ToListAsync()).Select(Map).ToList();
    public async Task<TutorResponseDto?> GetByIdAsync(int id) {
        var e = await _db.Tutores.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return e is null ? null : Map(e);
    }
    public async Task<TutorResponseDto?> CreateAsync(TutorCreateDto dto) {
        if (await _db.Tutores.AnyAsync(t => t.Email == dto.Email)) return null;
        var e = new Tutor { NomeCompleto = dto.NomeCompleto, Email = dto.Email, Telefone = dto.Telefone, Endereco = dto.Endereco, Cidade = dto.Cidade, Estado = dto.Estado };
        _db.Tutores.Add(e);
        await _db.SaveChangesAsync();
        
        return Map(e);
    }
    public async Task<bool> UpdateAsync(int id, TutorUpdateDto dto) {
        var e = await _db.Tutores.FindAsync(id);
        if (e is null) return false;
        e.NomeCompleto = dto.NomeCompleto;
        e.Email = dto.Email;
        e.Telefone = dto.Telefone;
        e.Endereco = dto.Endereco;
        e.Cidade = dto.Cidade;
        e.Estado = dto.Estado;
        await _db.SaveChangesAsync();
        return true;
    }
    public async Task<bool> DeleteAsync(int id) {
        var e = await _db.Tutores.FindAsync(id);
        if (e is null) return false;
        _db.Tutores.Remove(e);
        await _db.SaveChangesAsync();
        return true;
    }
    public async Task<IReadOnlyList<TutorResponseDto>> FindByCidadeAsync(string cidade) =>
        (await _db.Tutores.AsNoTracking().Where(e => e.Cidade.ToLower() == cidade.ToLower()).ToListAsync()).Select(Map).ToList();
    public async Task<TutorResponseDto?> FindByEmailAsync(string email) {
        var e = await _db.Tutores.AsNoTracking().FirstOrDefaultAsync(t => t.Email.ToLower() == email.ToLower());
        return e is null ? null : Map(e);
    }
    public async Task<IReadOnlyList<PetResponseDto>?> FindPetsAsync(int id) {
        if (!await _db.Tutores.AnyAsync(t => t.Id == id)) return null;
        var pets = await _db.Pets.Include(p => p.Tutor).Where(p => p.TutorId == id).AsNoTracking().ToListAsync();
        return pets.Select(p => new PetResponseDto(p.Id, p.Nome, p.Especie, p.Raca, p.DataNascimento, p.Sexo, p.PesoKg, p.FaseDeVida, p.TutorId, p.Tutor!.NomeCompleto)).ToList();
    }
}
