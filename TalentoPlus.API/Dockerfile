FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar archivos csproj y restaurar
COPY ["TalentoPlus.API/TalentoPlus.API.csproj", "TalentoPlus.API/"]
COPY ["TalentoPlus.Infrastructure/TalentoPlus.Infrastructure.csproj", "TalentoPlus.Infrastructure/"]
COPY ["TalentoPlus.Core/TalentoPlus.Core.csproj", "TalentoPlus.Core/"]

RUN dotnet restore "./TalentoPlus.API/TalentoPlus.API.csproj"

# Copiar todo y compilar
COPY . .
WORKDIR "/src/TalentoPlus.API"
RUN dotnet build "./TalentoPlus.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TalentoPlus.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TalentoPlus.API.dll"]