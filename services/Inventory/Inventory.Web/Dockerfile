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
COPY services/Platform/Platform.Web/Platform.Web.csproj services/Platform/Platform.Web/
COPY services/Platform/Platform.Application/Platform.Application.csproj services/Platform/Platform.Application/
COPY services/Platform/Platform.Infrastructure/Platform.Infrastructure.csproj services/Platform/Platform.Infrastructure/
COPY services/Platform/Platform.Domain/Platform.Domain.csproj services/Platform/Platform.Domain/
COPY shared/BusinessAsUsual.Shared.UI/BusinessAsUsual.Shared.UI.csproj shared/BusinessAsUsual.Shared.UI/
COPY BusinessAsUsual.Core/BusinessAsUsual.Core.csproj BusinessAsUsual.Core/

# Restore
RUN dotnet restore services/Inventory/Inventory.Web/Inventory.Web.csproj

# Copy source
COPY services/Inventory/Inventory.Web services/Inventory/Inventory.Web
COPY services/Inventory/Inventory.Application services/Inventory/Inventory.Application
COPY services/Inventory/Inventory.Domain services/Inventory/Inventory.Domain
COPY services/Inventory/Inventory.Infrastructure services/Inventory/Inventory.Infrastructure
COPY services/Platform/Platform.Web services/Platform/Platform.Web
COPY services/Platform/Platform.Application services/Platform/Platform.Application
COPY services/Platform/Platform.Infrastructure services/Platform/Platform.Infrastructure
COPY services/Platform/Platform.Domain services/Platform/Platform.Domain
COPY shared/BusinessAsUsual.Shared.UI shared/BusinessAsUsual.Shared.UI
COPY BusinessAsUsual.Core BusinessAsUsual.Core

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
