# ============================
# BUILD STAGE
# ============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files
COPY frontend/BusinessAsUsual.Admin/BusinessAsUsual.Admin.csproj frontend/BusinessAsUsual.Admin/
COPY backend/BusinessAsUsual.Domain/BusinessAsUsual.Domain.csproj backend/BusinessAsUsual.Domain/
COPY backend/BusinessAsUsual.Application/BusinessAsUsual.Application.csproj backend/BusinessAsUsual.Application/
COPY backend/BusinessAsUsual.Infrastructure/BusinessAsUsual.Infrastructure.csproj backend/BusinessAsUsual.Infrastructure/
COPY BusinessAsUsual.Core/BusinessAsUsual.Core.csproj BusinessAsUsual.Core/

# Restore
RUN dotnet restore frontend/BusinessAsUsual.Admin/BusinessAsUsual.Admin.csproj

# Copy everything
COPY frontend/BusinessAsUsual.Admin frontend/BusinessAsUsual.Admin
COPY backend/BusinessAsUsual.Domain backend/BusinessAsUsual.Domain
COPY backend/BusinessAsUsual.Application backend/BusinessAsUsual.Application
COPY backend/BusinessAsUsual.Infrastructure backend/BusinessAsUsual.Infrastructure
COPY BusinessAsUsual.Core BusinessAsUsual.Core

# Publish
RUN dotnet publish frontend/BusinessAsUsual.Admin/BusinessAsUsual.Admin.csproj -c Release -o /app/publish

# ============================
# RUNTIME STAGE
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:80

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BusinessAsUsual.Admin.dll"]