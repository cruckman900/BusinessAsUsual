# ============================
# BUILD STAGE
# ============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files (respecting the Inventory dependency graph) for a cached restore
COPY services/Inventory/Inventory.Web/Inventory.Web.csproj services/Inventory/Inventory.Web/
COPY services/Inventory/Inventory.Application/Inventory.Application.csproj services/Inventory/Inventory.Application/
COPY services/Inventory/Inventory.Domain/Inventory.Domain.csproj services/Inventory/Inventory.Domain/
COPY services/Inventory/Inventory.Infrastructure/Inventory.Infrastructure.csproj services/Inventory/Inventory.Infrastructure/

# Restore
RUN dotnet restore services/Inventory/Inventory.Web/Inventory.Web.csproj

# Copy source
COPY services/Inventory/Inventory.Web services/Inventory/Inventory.Web
COPY services/Inventory/Inventory.Application services/Inventory/Inventory.Application
COPY services/Inventory/Inventory.Domain services/Inventory/Inventory.Domain
COPY services/Inventory/Inventory.Infrastructure services/Inventory/Inventory.Infrastructure

# Publish
RUN dotnet publish services/Inventory/Inventory.Web/Inventory.Web.csproj -c Release -o /app/publish

# ============================
# RUNTIME STAGE
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Inventory.Web.dll"]
