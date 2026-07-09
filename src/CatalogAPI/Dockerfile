FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/CatalogAPI/CatalogAPI.csproj", "src/CatalogAPI/"]
COPY ["src/CatalogAPI.Application/CatalogAPI.Application.csproj", "src/CatalogAPI.Application/"]
COPY ["src/CatalogAPI.Domain/CatalogAPI.Domain.csproj", "src/CatalogAPI.Domain/"]
COPY ["src/CatalogAPI.Infrastructure/CatalogAPI.Infrastructure.csproj", "src/CatalogAPI.Infrastructure/"]
COPY ["src/CatalogAPI.Shared/CatalogAPI.Shared.csproj", "src/CatalogAPI.Shared/"]

RUN dotnet restore "src/CatalogAPI/CatalogAPI.csproj"

COPY . .

WORKDIR "/src/src/CatalogAPI"

RUN dotnet publish "CatalogAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "CatalogAPI.dll"]