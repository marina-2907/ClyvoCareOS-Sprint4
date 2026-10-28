# Arquitetura ClyvoCare OS

```mermaid
flowchart TD
  Client[Cliente HTTP] --> Api[API ASP.NET Core / Swagger / JWT]
  Api --> App[Application: contratos, DTOs, serviços]
  App --> Domain[Domain: Pet, Tutor, Consulta, Vacina, Tratamento]
  Api --> Infra[Infrastructure: persistência]
  Infra --> App
  Infra --> Domain
  Infra --> Oracle[(Oracle 19c / EF Core)]
  Infra --> Mongo[(MongoDB / historico_atividades)]
  Api --> Obs[Serilog / OpenTelemetry / Health Checks]
```

## Limitação arquitetural identificada

O projeto foi fisicamente separado em assemblies com orientação correta das dependências entre Domain, Application e Infrastructure. Entretanto, **controllers legados** ainda utilizam `AppDbContext` diretamente para operações CRUD; isso deve ser migrado para casos de uso e interfaces da Application para alcançar Clean Architecture rigorosa. O `PetsController` já delega as operações GET principais ao `PetQueryService`. O endpoint Mongo usa contrato da Application. Não declarar 100% de Clean Architecture antes dessa migração.
