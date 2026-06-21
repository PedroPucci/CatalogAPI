using CatalogAPI.Application.Abstractions.Persistence;
using CatalogAPI.Application.Abstractions.Repositories;
using CatalogAPI.Extensions.SwaggerDocumentation;
using CatalogAPI.Infrastructure.Connections;
using CatalogAPI.Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

namespace CatalogAPI.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddEndpointsApiExplorer();

            var jwtSettings = config.GetSection("JwtSettings");
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var secretKey = jwtSettings["SecretKey"];

            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseSqlServer(config.GetConnectionString("WebApiDatabase"));
            });

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = issuer,
                    ValidAudience = audience,

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secretKey!)),

                    ClockSkew = TimeSpan.FromMinutes(5),
                    RoleClaimType = ClaimTypes.Role
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";

                        return context.Response.WriteAsync(
                            "{\"success\": false, \"message\": \"Invalid or expired token. Please log in again.\"}");
                    },

                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = 403;
                        context.Response.ContentType = "application/json";

                        return context.Response.WriteAsync(
                            "{\"success\": false, \"message\": \"You do not have permission to access this resource.\"}");
                    }
                };
            });

            services.AddAuthorization();

            services.AddSwaggerGen(opt =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                opt.CustomSchemaIds(t => t.FullName);
                opt.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                opt.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "FCG Catalog API",
                    Description = @"
                        Microsserviço responsável pelo gerenciamento do catálogo de jogos da plataforma FCG.

                        Funcionalidades:
                        - Cadastro de jogos.
                        - Consulta de jogos.
                        - Atualização de jogos.
                        - Remoção lógica de jogos.
                        - Controle de acesso ao CRUD de jogos por perfil administrativo.
                        - Gerenciamento da biblioteca de jogos do usuário.
                        - Inicialização do fluxo de compra de jogos.
                        - Publicação de eventos para integração entre microsserviços.

                        Desenvolvido em .NET 8 utilizando Clean Architecture, com foco em escalabilidade, manutenção e desacoplamento entre serviços.
                    "
                });

                opt.OperationFilter<CustomOperationDescriptions>();

                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Cole apenas o token JWT, sem escrever Bearer."
                });

                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                if (File.Exists(xmlPath))
                {
                    opt.IncludeXmlComments(xmlPath);
                }
            });

            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithOrigins("http://localhost:4200");
                });
            });

            services.AddScoped<IRepositoryUoW, RepositoryUoW>();
            services.AddScoped<IUnitOfWorkService, UnitOfWorkService>();
            services.AddScoped<IGameRepository, GameRepository>();

            services.AddMvc().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

            return services;
        }
    }
}