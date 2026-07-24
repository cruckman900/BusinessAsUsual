# ============================
# BUILD STAGE
# ============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files (respecting the HR dependency graph) for a cached restore
COPY services/HR/HR.Web/HR.Web.csproj services/HR/HR.Web/
COPY services/HR/HR.Application/HR.Application.csproj services/HR/HR.Application/
COPY services/HR/HR.Infrastructure/HR.Infrastructure.csproj services/HR/HR.Infrastructure/
COPY services/HR/HR.Domain/HR.Domain.csproj services/HR/HR.Domain/
COPY BusinessAsUsual.Core/BusinessAsUsual.Core.csproj BusinessAsUsual.Core/

# Restore
RUN dotnet restore services/HR/HR.Web/HR.Web.csproj

# Copy source
COPY services/HR/HR.Web services/HR/HR.Web
COPY services/HR/HR.Application services/HR/HR.Application
COPY services/HR/HR.Infrastructure services/HR/HR.Infrastructure
COPY services/HR/HR.Domain services/HR/HR.Domain
COPY BusinessAsUsual.Core BusinessAsUsual.Core

# Publish
RUN dotnet publish services/HR/HR.Web/HR.Web.csproj -c Release -o /app/publish

# ============================
# RUNTIME STAGE
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HR.Web.dll"]
