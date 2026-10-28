namespace ClyvoCareOSChallenge.Application.Contracts;

public sealed record AtividadeHistoricoDto(string Id, int PetId, string Tipo, string Descricao, DateTime DataRegistro);

public interface IHistoricoRepository
{
    Task<AtividadeHistoricoDto> CriarAsync(int petId, string tipo, string descricao, CancellationToken ct = default);
    Task<IReadOnlyList<AtividadeHistoricoDto>> ListarPorPetAsync(int petId, CancellationToken ct = default);
}
