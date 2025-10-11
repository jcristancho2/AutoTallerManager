# ---------- Runtime base ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
# ---------- Build ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiamos primero los .csproj para aprovechar caché de Docker
COPY ["AutoTallerManager.API/AutoTallerManager.API.csproj", "AutoTallerManager.API/"]
COPY ["AutoTallerManager.Application/AutoTallerManager.Application.csproj", "AutoTallerManager.Application/"]
COPY ["AutoTallerManager.Infrastructure/AutoTallerManager.Infrastructure.csproj", "AutoTallerManager.Infrastructure/"]
COPY ["AutoTallerManager.Domain/AutoTallerManager.Domain.csproj", "AutoTallerManager.Domain/"]

# Restauramos dependencias
RUN dotnet restore "AutoTallerManager.API/AutoTallerManager.API.csproj"

# Copiamos el resto del código
COPY . .

# Compilamos el proyecto en modo Release
WORKDIR "/src/AutoTallerManager.API"
RUN dotnet build "AutoTallerManager.API.csproj" -c Release -o /app/build

# ---------- Publish ----------
FROM build AS publish
RUN dotnet publish "AutoTallerManager.API.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

# ---------- Final ----------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Definimos el punto de entrada de la aplicación
ENTRYPOINT ["dotnet", "AutoTallerManager.API.dll"]


