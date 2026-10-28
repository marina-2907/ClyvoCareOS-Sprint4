using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClyvoCareOSChallenge.Infrastructure.MongoDB;

public sealed class HistoricoAtividade
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public int PetId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataRegistro { get; set; } = DateTime.UtcNow;
}
