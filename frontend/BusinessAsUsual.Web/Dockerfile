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
# Shared core (event bus, integration events)
COPY BusinessAsUsual.Core/BusinessAsUsual.Core.csproj BusinessAsUsual.Core/

# Restore
RUN dotnet restore frontend/BusinessAsUsual.Web/BusinessAsUsual.Web.csproj

# Copy everything
COPY frontend/BusinessAsUsual.Web frontend/BusinessAsUsual.Web
COPY backend/BusinessAsUsual.Domain backend/BusinessAsUsual.Domain
COPY backend/BusinessAsUsual.Application backend/BusinessAsUsual.Application
COPY backend/BusinessAsUsual.Infrastructure backend/BusinessAsUsual.Infrastructure
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