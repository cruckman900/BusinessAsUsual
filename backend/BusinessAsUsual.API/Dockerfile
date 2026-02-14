# ============================
# BUILD STAGE
# ============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files
COPY backend/BusinessAsUsual.API/BusinessAsUsual.API.csproj backend/BusinessAsUsual.API/
COPY backend/BusinessAsUsual.Domain/BusinessAsUsual.Domain.csproj backend/BusinessAsUsual.Domain/
COPY backend/BusinessAsUsual.Application/BusinessAsUsual.Application.csproj backend/BusinessAsUsual.Application/
COPY backend/BusinessAsUsual.Infrastructure/BusinessAsUsual.Infrastructure.csproj backend/BusinessAsUsual.Infrastructure/

# Restore
RUN dotnet restore backend/BusinessAsUsual.API/BusinessAsUsual.API.csproj

# Copy everything
COPY backend/BusinessAsUsual.API backend/BusinessAsUsual.API
COPY backend/BusinessAsUsual.Domain backend/BusinessAsUsual.Domain
COPY backend/BusinessAsUsual.Application backend/BusinessAsUsual.Application
COPY backend/BusinessAsUsual.Infrastructure backend/BusinessAsUsual.Infrastructure

# Publish
RUN dotnet publish backend/BusinessAsUsual.API/BusinessAsUsual.API.csproj -c Release -o /app/publish

# ============================
# RUNTIME STAGE
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BusinessAsUsual.API.dll"]