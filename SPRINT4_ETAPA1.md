# Sprint 4 — Etapa 1 (implementação inicial)

## Entregue nesta etapa
- Sanitização da senha Oracle que estava em `appsettings.json` (configurar via Secret Manager ou variável de ambiente).
- Middleware global para exceções inesperadas, com HTTP 500 e `ProblemDetails`, sem vazar stack trace.
- `GET /api/pets` com filtros `nome` e `especie`, paginação (`page`, `pageSize` limitado a 100), ordenação (`sortBy=id|nome|especie`, `order=asc|desc`) e links de navegação `self`, `prev` e `next`.
- CRUDs e testes anteriores preservados no restante do projeto.

## Executar localmente (PowerShell)
```powershell
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:OracleConnection" "User Id=SEU_USUARIO;Password=SUA_SENHA;Data Source=SEU_HOST:1521/ORCL;"
dotnet restore
dotnet test
dotnet run
```
Alternativa de conexão: variável de ambiente `ConnectionStrings__OracleConnection`.

**Segurança:** a cópia original continha credencial Oracle em texto simples. Troque essa senha e retire a credencial também de repositórios/histórico, caso tenha sido publicada.

## Pendências antes da Sprint 4 final
- Refatorar em projetos independentes Domain / Application / Infrastructure / Api e aplicar Repository Pattern.
- JWT com autorização e endpoints protegidos.
- MongoDB com operações persistentes e health check.
- Testes das regras de negócio e integração com banco, cobertura e confirmação de migrations.
- Revisar Swagger/OpenAPI para todos os contratos e gerar diagrama/README final.

**Validação:** não foi possível executar `dotnet build` ou `dotnet test` neste ambiente, pois o SDK .NET não está instalado.
