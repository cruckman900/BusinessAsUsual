# ============================
# BUILD STAGE
# ============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files (respecting the Sales dependency graph) for a cached restore
COPY services/Sales/Sales.Web/Sales.Web.csproj services/Sales/Sales.Web/
COPY services/Sales/Sales.Application/Sales.Application.csproj services/Sales/Sales.Application/
COPY services/Sales/Sales.Domain/Sales.Domain.csproj services/Sales/Sales.Domain/
COPY services/Sales/Sales.Infrastructure/Sales.Infrastructure.csproj services/Sales/Sales.Infrastructure/
COPY services/Sales/Sales.Contracts/Sales.Contracts.csproj services/Sales/Sales.Contracts/
COPY services/Platform/Platform.Web/Platform.Web.csproj services/Platform/Platform.Web/
COPY services/CRM/CRM.Application/CRM.Application.csproj services/CRM/CRM.Application/
COPY services/CRM/CRM.Domain/CRM.Domain.csproj services/CRM/CRM.Domain/
COPY services/Inventory/Inventory.Application/Inventory.Application.csproj services/Inventory/Inventory.Application/
COPY services/Inventory/Inventory.Domain/Inventory.Domain.csproj services/Inventory/Inventory.Domain/
COPY shared/BusinessAsUsual.Shared.UI/BusinessAsUsual.Shared.UI.csproj shared/BusinessAsUsual.Shared.UI/
COPY services/HR/HR.Application/HR.Application.csproj services/HR/HR.Application/
COPY services/HR/HR.Domain/HR.Domain.csproj services/HR/HR.Domain/
COPY services/HR/HR.Infrastructure/HR.Infrastructure.csproj services/HR/HR.Infrastructure/
COPY BusinessAsUsual.Core/BusinessAsUsual.Core.csproj BusinessAsUsual.Core/

# Restore
RUN dotnet restore services/Sales/Sales.Web/Sales.Web.csproj

# Copy source
COPY services/Sales/Sales.Web services/Sales/Sales.Web
COPY services/Sales/Sales.Application services/Sales/Sales.Application
COPY services/Sales/Sales.Domain services/Sales/Sales.Domain
COPY services/Sales/Sales.Infrastructure services/Sales/Sales.Infrastructure
COPY services/Sales/Sales.Contracts services/Sales/Sales.Contracts
COPY services/Platform/Platform.Web services/Platform/Platform.Web
COPY services/CRM/CRM.Application services/CRM/CRM.Application
COPY services/CRM/CRM.Domain services/CRM/CRM.Domain
COPY services/Inventory/Inventory.Application services/Inventory/Inventory.Application
COPY services/Inventory/Inventory.Domain services/Inventory/Inventory.Domain
COPY shared/BusinessAsUsual.Shared.UI shared/BusinessAsUsual.Shared.UI
COPY services/HR/HR.Application services/HR/HR.Application
COPY services/HR/HR.Domain services/HR/HR.Domain
COPY services/HR/HR.Infrastructure services/HR/HR.Infrastructure
COPY BusinessAsUsual.Core BusinessAsUsual.Core

# Publish
RUN dotnet publish services/Sales/Sales.Web/Sales.Web.csproj -c Release -o /app/publish

# ============================
# RUNTIME STAGE
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Sales.Web.dll"]
