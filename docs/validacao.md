# Validação local

1. `dotnet restore`
2. `dotnet build`
3. `dotnet test --collect:"XPlat Code Coverage"`
4. `dotnet ef migrations list --project src/ClyvoCare.Infrastructure --startup-project .`
5. `dotnet run --launch-profile https`
6. Swagger na URL HTTPS informada pela API + `/swagger`.
7. Testar JWT (`POST /api/auth/login`, `GET /api/auth/validar`), MongoDB (`POST /api/historico-atividades`, `GET /api/historico-atividades/pet/{id}`) e CRUD Oracle.

## Requisitos de configuração

Usar `dotnet user-secrets` no projeto API para `ConnectionStrings:OracleConnection`, `MongoDB:ConnectionString`, `MongoDB:DatabaseName`, `Jwt:Issuer`, `Jwt:Audience`, `Jwt:Key`, `DemoAuth:Password`, `Authentication:ApiKey`. Nunca versionar segredos.

**Cuidado:** migrations criadas com `ClyvoCare.Infrastructure` devem ser manipuladas com `--project src/ClyvoCare.Infrastructure --startup-project .`. Nunca aplicar `database update` em esquemas não autorizados.

Observação: a compilação e os testes deste pacote precisam ser realizados num computador com .NET SDK 8+ e bancos configurados; não foram executados neste ambiente.
