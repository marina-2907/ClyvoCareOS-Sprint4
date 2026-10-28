using ClyvoCareOSChallenge.Authentication;
using ClyvoCareOSChallenge.Middleware;
using Microsoft.AspNetCore.Authentication;
using System.Reflection;
using ClyvoCareOSChallenge.Data;
using ClyvoCareOSChallenge.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using ClyvoCareOSChallenge.Infrastructure.Repositories;

public partial class Program
{
    private static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Define as informaÃ§Ãµes exibidas em cada linha do log.
        const string logOutputTemplate =
            "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] " +
            "[CorrelationId: {CorrelationId}] " +
            "{Message:lj}{NewLine}{Exception}";

        // Configura os logs no terminal e em arquivos diÃƒÂ¡rios.
        builder.Host.UseSerilog((context, services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "ClyvoCareOSChallenge")
                .WriteTo.Console(
                    outputTemplate: logOutputTemplate
                )
                .WriteTo.File(
                    "logs/clyvocare-.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    outputTemplate: logOutputTemplate
                );
        });

        // Adiciona os controllers utilizados pelos endpoints da API.
        builder.Services.AddControllers();

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    ApiKeyAuthenticationHandler.SchemeName;

                options.DefaultChallengeScheme =
                    ApiKeyAuthenticationHandler.SchemeName;
            })
            .AddScheme<
                ApiKeyAuthenticationOptions,
                ApiKeyAuthenticationHandler
            >(
                ApiKeyAuthenticationHandler.SchemeName,
                options =>
                {
                    options.ApiKey =
                        builder.Configuration[
                            "Authentication:ApiKey"
                        ] ?? string.Empty;
                }
            );


        builder.Services.AddAuthentication()
            .AddJwtBearer(
                Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    var key = builder.Configuration["Jwt:Key"];

                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        options.TokenValidationParameters =
                            new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                            {
                                ValidateIssuer = true,
                                ValidateAudience = true,
                                ValidateLifetime = true,
                                ValidateIssuerSigningKey = true,
                                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                                ValidAudience = builder.Configuration["Jwt:Audience"],
                                IssuerSigningKey =
                                    new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                                        Convert.FromBase64String(key)),
                                ClockSkew = TimeSpan.FromSeconds(30)
                            };
                    }
                });
        builder.Services.AddSingleton<MongoDB.Driver.IMongoClient>(sp =>
        {
            IConfiguration configuration = sp.GetRequiredService<IConfiguration>();
            var connectionString = configuration["MongoDB:ConnectionString"]
                ?? throw new InvalidOperationException("MongoDB nÃ£o configurado.");

            return new MongoDB.Driver.MongoClient(connectionString);
        });

        builder.Services.AddSingleton(sp =>
        {
            IConfiguration configuration = sp.GetRequiredService<IConfiguration>();
            var databaseName = configuration["MongoDB:DatabaseName"]
                ?? throw new InvalidOperationException("Nome do banco MongoDB nÃ£o configurado.");

            return sp.GetRequiredService<MongoDB.Driver.IMongoClient>()
                .GetDatabase(databaseName);
        });

        builder.Services.AddScoped<
            ClyvoCareOSChallenge.Application.Contracts.IHistoricoRepository,
            ClyvoCareOSChallenge.Infrastructure.MongoDB.HistoricoRepository>();
        builder.Services.AddAuthorization();

        var serviceCollection = builder.Services.AddScoped<
            ClyvoCareOSChallenge.Application.Contracts.IPetReadRepository,
            PetReadRepository>();

        builder.Services.AddScoped<
            ClyvoCareOSChallenge.Application.PetQueryService>();

        builder.Services.AddScoped<
            ClyvoCareOSChallenge.Application.Contracts.IPetManagementRepository,
            ClyvoCareOSChallenge.Infrastructure.Repositories.PetManagementRepository>();

        builder.Services.AddScoped<
            ClyvoCareOSChallenge.Application.PetManagementService>();


        // Configura a conexÃ£o da aplicaÃ§Ã£o com o banco de dados Oracle.
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseOracle(
                builder.Configuration.GetConnectionString("OracleConnection"),
                oracleOptions =>
                {
                    oracleOptions.UseOracleSQLCompatibility(
                        OracleSQLCompatibility.DatabaseVersion19
                    );
                    oracleOptions.MigrationsAssembly("ClyvoCare.Infrastructure");
                }
            );
        });

        // Configura o cliente HTTP usado para verificar o ViaCEP.
        builder.Services.AddHttpClient("ViaCep", client =>
        {
            client.BaseAddress = new Uri("https://viacep.com.br");
            client.Timeout = TimeSpan.FromSeconds(5);
        });

        // Registra as verificaÃ§Ãµes de saÃºde da API, Oracle e ViaCEP.
        builder.Services
            .AddHealthChecks()

            // Verifica somente se a aplicaÃ§Ã£o estÃƒÂ¡ em execuÃ§Ã£o.
            .AddCheck(
                "api",
                () => HealthCheckResult.Healthy("API funcionando."),
                tags: new[] { "live" }
            )

            // Verifica se a aplicaÃ§Ã£o consegue acessar o Oracle.
            .AddDbContextCheck<AppDbContext>(
                "oracle-database",
                tags: new[] { "ready", "database" }
            )

            // Verifica se o serviÃƒÂ§o externo ViaCEP estÃƒÂ¡ disponÃ­vel.
            .AddCheck<ExternalServiceHealthCheck>(
                "viacep-api",
                tags: new[] { "ready", "external" }
            );

        // Configura tracing e mÃ©tricas da aplicaÃ§Ã£o.
        builder.Services
            .AddOpenTelemetry()

            // Identifica a aplicaÃ§Ã£o que gerou os dados.
            .ConfigureResource(resource =>
            {
                resource.AddService("ClyvoCareOSChallenge.Api");
            })

            // Rastreia as requisiÃƒÂ§ÃƒÂµes recebidas e chamadas HTTP externas.
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddConsoleExporter();
            })

            // Coleta mÃ©tricas da API, chamadas HTTP e execuÃ§Ã£o do .NET.
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddPrometheusExporter();
            });

        // Configura a documentaÃ§Ã£o da API com Swagger.
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "ClyvoCare OS API",
                Version = "v1",
                Description =
                    "API para gerenciamento de saÃºde animal: vacinas, consultas e tratamentos."
            });

            // Carrega no Swagger os comentÃ¡rios XML escritos nos endpoints.
            var xmlFile =
                $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

            var xmlPath =
                Path.Combine(AppContext.BaseDirectory, xmlFile);

            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
        });

        // Permite que o front-end acesse a API durante o desenvolvimento.
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
builder.Services.AddScoped<ClyvoCareOSChallenge.Application.Contracts.ITutorRepository,
    ClyvoCareOSChallenge.Infrastructure.Repositories.TutorRepository>();
builder.Services.AddScoped<ClyvoCareOSChallenge.Application.TutorService>();
builder.Services.AddScoped<ClyvoCareOSChallenge.Application.Contracts.IVacinaRepository,
    ClyvoCareOSChallenge.Infrastructure.Repositories.VacinaRepository>();
builder.Services.AddScoped<ClyvoCareOSChallenge.Application.VacinaService>();
builder.Services.AddScoped<ClyvoCareOSChallenge.Application.Contracts.IConsultaRepository,
    ClyvoCareOSChallenge.Infrastructure.Repositories.ConsultaRepository>();
builder.Services.AddScoped<ClyvoCareOSChallenge.Application.ConsultaService>();
builder.Services.AddScoped<ClyvoCareOSChallenge.Application.Contracts.ITratamentoRepository,
    ClyvoCareOSChallenge.Infrastructure.Repositories.TratamentoRepository>();
builder.Services.AddScoped<ClyvoCareOSChallenge.Application.TratamentoService>();


        WebApplication app = builder.Build();

        app.UseMiddleware<GlobalExceptionMiddleware>();

        // Cria um identificador para acompanhar cada requisiÃƒÂ§ÃƒÂ£o nos logs.
        app.Use(async (context, next) =>
        {
            const string headerName = "X-Correlation-ID";

            var correlationId =
                context.Request.Headers[headerName].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
            }

            context.Response.Headers[headerName] = correlationId;

            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                await next();
            }
        });

        // Registra cada requisiÃƒÂ§ÃƒÂ£o e escolhe o nÃ­vel conforme o resultado.
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate =
                "HTTP {RequestMethod} {RequestPath} respondeu " +
                "{StatusCode} em {Elapsed:0.0000} ms";

            options.GetLevel = (httpContext, elapsed, exception) =>
            {
                // ExceÃ§Ãµes e respostas 500 sÃƒÂ£o registradas como Error.
                if (exception is not null ||
                    httpContext.Response.StatusCode >=
                    StatusCodes.Status500InternalServerError)
                {
                    return LogEventLevel.Error;
                }

                // Respostas 400 sÃƒÂ£o registradas como Warning.
                if (httpContext.Response.StatusCode >=
                    StatusCodes.Status400BadRequest)
                {
                    return LogEventLevel.Warning;
                }

                // RequisiÃ§Ãµes bem-sucedidas sÃƒÂ£o Information.
                return LogEventLevel.Information;
            };
        });

        // Disponibiliza o Swagger na pÃ¡gina inicial.
        app.UseSwagger();

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint(
                "/swagger/v1/swagger.json",
                "ClyvoCare OS API v1"
            );

            options.RoutePrefix = string.Empty;
        });

        app.UseHttpsRedirection();
        app.UseCors("AllowAll");

        // Primeiro identifica e autentica o cliente.
        app.UseAuthentication();

        // Depois verifica se ele possui autorizaÃƒÂ§ÃƒÂ£o.
        app.UseAuthorization();

        app.MapControllers();

        // Executa todas as verificaÃ§Ãµes cadastradas.
        app.MapHealthChecks("/health");

        // Verifica somente se a API estÃƒÂ¡ funcionando.
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = healthCheck =>
                healthCheck.Tags.Contains("live")
        });

        // Verifica Oracle e ViaCEP.
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = healthCheck =>
                healthCheck.Tags.Contains("ready")
        });

        // Disponibiliza as mÃ©tricas coletadas pelo Prometheus.
        app.MapPrometheusScrapingEndpoint();

        app.Run();
    }
}




