# ============================
# BUILD STAGE
# ============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files
COPY frontend/BusinessAsUsual.Web/BusinessAsUsual.Web.csproj frontend/BusinessAsUsual.Web/
COPY backend/BusinessAsUsual.Domain/BusinessAsUsual.Domain.csproj backend/BusinessAsUsual.Domain/
COPY backend/BusinessAsUsual.Application/BusinessAsUsual.Application.csproj backend/BusinessAsUsual.Application/
COPY backend/BusinessAsUsual.Infrastructure/BusinessAsUsual.Infrastructure.csproj backend/BusinessAsUsual.Infrastructure/
# Shared UI library
COPY shared/BusinessAsUsual.Shared.UI/BusinessAsUsual.Shared.UI.csproj shared/BusinessAsUsual.Shared.UI/
# HR service projects referenced by the web shell
COPY services/HR/HR.Domain/HR.Domain.csproj services/HR/HR.Domain/
COPY services/HR/HR.Application/HR.Application.csproj services/HR/HR.Application/
COPY services/HR/HR.Infrastructure/HR.Infrastructure.csproj services/HR/HR.Infrastructure/
COPY services/HR/HR.Web/HR.Web.csproj services/HR/HR.Web/
# CRM service projects referenced by the web shell
COPY services/CRM/CRM.Domain/CRM.Domain.csproj services/CRM/CRM.Domain/
COPY services/CRM/CRM.Application/CRM.Application.csproj services/CRM/CRM.Application/
COPY services/CRM/CRM.Web/CRM.Web.csproj services/CRM/CRM.Web/
# Finance service projects referenced by the web shell
COPY services/Finance/Finance.Domain/Finance.Domain.csproj services/Finance/Finance.Domain/
COPY services/Finance/Finance.Application/Finance.Application.csproj services/Finance/Finance.Application/
COPY services/Finance/Finance.Web/Finance.Web.csproj services/Finance/Finance.Web/
# Inventory service projects referenced by the web shell
COPY services/Inventory/Inventory.Domain/Inventory.Domain.csproj services/Inventory/Inventory.Domain/
COPY services/Inventory/Inventory.Application/Inventory.Application.csproj services/Inventory/Inventory.Application/
COPY services/Inventory/Inventory.Infrastructure/Inventory.Infrastructure.csproj services/Inventory/Inventory.Infrastructure/
COPY services/Inventory/Inventory.Web/Inventory.Web.csproj services/Inventory/Inventory.Web/
# Sales service projects referenced by the web shell
COPY services/Sales/Sales.Domain/Sales.Domain.csproj services/Sales/Sales.Domain/
COPY services/Sales/Sales.Application/Sales.Application.csproj services/Sales/Sales.Application/
COPY services/Sales/Sales.Infrastructure/Sales.Infrastructure.csproj services/Sales/Sales.Infrastructure/
COPY services/Sales/Sales.Contracts/Sales.Contracts.csproj services/Sales/Sales.Contracts/
COPY services/Sales/Sales.Web/Sales.Web.csproj services/Sales/Sales.Web/
# Services service projects referenced by the web shell
COPY services/Services/Services.Domain/Services.Domain.csproj services/Services/Services.Domain/
COPY services/Services/Services.Application/Services.Application.csproj services/Services/Services.Application/
COPY services/Services/Services.Infrastructure/Services.Infrastructure.csproj services/Services/Services.Infrastructure/
COPY services/Services/Services.Contracts/Services.Contracts.csproj services/Services/Services.Contracts/
COPY services/Services/Services.Web/Services.Web.csproj services/Services/Services.Web/
# Platform service projects referenced by the web shell
COPY services/Platform/Platform.Domain/Platform.Domain.csproj services/Platform/Platform.Domain/
COPY services/Platform/Platform.Application/Platform.Application.csproj services/Platform/Platform.Application/
COPY services/Platform/Platform.Infrastructure/Platform.Infrastructure.csproj services/Platform/Platform.Infrastructure/
COPY services/Platform/Platform.Contracts/Platform.Contracts.csproj services/Platform/Platform.Contracts/
COPY services/Platform/Platform.Web/Platform.Web.csproj services/Platform/Platform.Web/
# LMS service projects referenced by the web shell
COPY services/LearningManagement/LMS.Domain/LMS.Domain.csproj services/LearningManagement/LMS.Domain/
COPY services/LearningManagement/LMS.Application/LMS.Application.csproj services/LearningManagement/LMS.Application/
COPY services/LearningManagement/LMS.Infrastructure/LMS.Infrastructure.csproj services/LearningManagement/LMS.Infrastructure/
COPY services/LearningManagement/LMS.API/LMS.API.csproj services/LearningManagement/LMS.API/
COPY services/LearningManagement/LMS.Contracts/LMS.Contracts.csproj services/LearningManagement/LMS.Contracts/
# Shared core (event bus, integration events)
COPY BusinessAsUsual.Core/BusinessAsUsual.Core.csproj BusinessAsUsual.Core/

# Restore
RUN dotnet restore frontend/BusinessAsUsual.Web/BusinessAsUsual.Web.csproj

# Copy everything
COPY frontend/BusinessAsUsual.Web frontend/BusinessAsUsual.Web
COPY backend/BusinessAsUsual.Domain backend/BusinessAsUsual.Domain
COPY backend/BusinessAsUsual.Application backend/BusinessAsUsual.Application
COPY backend/BusinessAsUsual.Infrastructure backend/BusinessAsUsual.Infrastructure
# Shared UI library source
COPY shared/BusinessAsUsual.Shared.UI shared/BusinessAsUsual.Shared.UI
# HR service source
COPY services/HR/HR.Domain services/HR/HR.Domain
COPY services/HR/HR.Application services/HR/HR.Application
COPY services/HR/HR.Infrastructure services/HR/HR.Infrastructure
COPY services/HR/HR.Web services/HR/HR.Web
# CRM service source
COPY services/CRM/CRM.Domain services/CRM/CRM.Domain
COPY services/CRM/CRM.Application services/CRM/CRM.Application
COPY services/CRM/CRM.Web services/CRM/CRM.Web
# Finance service source
COPY services/Finance/Finance.Domain services/Finance/Finance.Domain
COPY services/Finance/Finance.Application services/Finance/Finance.Application
COPY services/Finance/Finance.Web services/Finance/Finance.Web
# Inventory service source
COPY services/Inventory/Inventory.Domain services/Inventory/Inventory.Domain
COPY services/Inventory/Inventory.Application services/Inventory/Inventory.Application
COPY services/Inventory/Inventory.Infrastructure services/Inventory/Inventory.Infrastructure
COPY services/Inventory/Inventory.Web services/Inventory/Inventory.Web
# Sales service source
COPY services/Sales/Sales.Domain services/Sales/Sales.Domain
COPY services/Sales/Sales.Application services/Sales/Sales.Application
COPY services/Sales/Sales.Infrastructure services/Sales/Sales.Infrastructure
COPY services/Sales/Sales.Contracts services/Sales/Sales.Contracts
COPY services/Sales/Sales.Web services/Sales/Sales.Web
# Services service source
COPY services/Services/Services.Domain services/Services/Services.Domain
COPY services/Services/Services.Application services/Services/Services.Application
COPY services/Services/Services.Infrastructure services/Services/Services.Infrastructure
COPY services/Services/Services.Contracts services/Services/Services.Contracts
COPY services/Services/Services.Web services/Services/Services.Web
# Platform service source
COPY services/Platform/Platform.Domain services/Platform/Platform.Domain
COPY services/Platform/Platform.Application services/Platform/Platform.Application
COPY services/Platform/Platform.Infrastructure services/Platform/Platform.Infrastructure
COPY services/Platform/Platform.Contracts services/Platform/Platform.Contracts
COPY services/Platform/Platform.Web services/Platform/Platform.Web
# LMS service source
COPY services/LearningManagement/LMS.Domain services/LearningManagement/LMS.Domain
COPY services/LearningManagement/LMS.Application services/LearningManagement/LMS.Application
COPY services/LearningManagement/LMS.Infrastructure services/LearningManagement/LMS.Infrastructure
COPY services/LearningManagement/LMS.API services/LearningManagement/LMS.API
COPY services/LearningManagement/LMS.Contracts services/LearningManagement/LMS.Contracts
# Shared core source
COPY BusinessAsUsual.Core BusinessAsUsual.Core

# Publish
RUN dotnet publish frontend/BusinessAsUsual.Web/BusinessAsUsual.Web.csproj -c Release -o /app/publish

# ============================
# RUNTIME STAGE
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:80

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BusinessAsUsual.Web.dll"]