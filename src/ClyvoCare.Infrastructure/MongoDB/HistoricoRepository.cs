using ClyvoCareOSChallenge.Application.Contracts;
using MongoDB.Driver;

namespace ClyvoCareOSChallenge.Infrastructure.MongoDB;

public sealed class HistoricoRepository : IHistoricoRepository
{
    private readonly IMongoCollection<HistoricoAtividade> _collection;
    public HistoricoRepository(IMongoDatabase database) =>
        _collection = database.GetCollection<HistoricoAtividade>("historico_atividades");

    private static AtividadeHistoricoDto Map(HistoricoAtividade a) =>
        new(a.Id, a.PetId, a.Tipo, a.Descricao, a.DataRegistro);

    public async Task<AtividadeHistoricoDto> CriarAsync(int petId, string tipo, string descricao, CancellationToken ct = default)
    {
        var registro = new HistoricoAtividade { PetId = petId, Tipo = tipo, Descricao = descricao, DataRegistro = DateTime.UtcNow };
        await _collection.InsertOneAsync(registro, cancellationToken: ct);
        return Map(registro);
    }

    public async Task<IReadOnlyList<AtividadeHistoricoDto>> ListarPorPetAsync(int petId, CancellationToken ct = default)
    {
        var registros = await _collection.Find(x => x.PetId == petId)
            .SortByDescending(x => x.DataRegistro).ToListAsync(ct);
        return registros.Select(Map).ToList();
    }
}
