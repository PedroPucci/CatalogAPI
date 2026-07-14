FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

USER app
WORKDIR /app

EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ["nuget.config", "/src/nuget.config"]
COPY ["nuget-packages/", "/src/nuget-packages/"]

COPY ["src/CatalogAPI/CatalogAPI.csproj", "src/CatalogAPI/"]
COPY ["CatalogAPI.Application/CatalogAPI.Application.csproj", "CatalogAPI.Application/"]
COPY ["CatalogAPI.Domain/CatalogAPI.Domain.csproj", "CatalogAPI.Domain/"]
COPY ["CatalogAPI.Infrastructure/CatalogAPI.Infrastructure.csproj", "CatalogAPI.Infrastructure/"]
COPY ["CatalogAPI.Shared/CatalogAPI.Shared.csproj", "CatalogAPI.Shared/"]

RUN dotnet restore "src/CatalogAPI/CatalogAPI.csproj" \
    --configfile "/src/nuget.config"

COPY . .

RUN dotnet publish "src/CatalogAPI/CatalogAPI.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false


FROM base AS final

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "CatalogAPI.dll"]