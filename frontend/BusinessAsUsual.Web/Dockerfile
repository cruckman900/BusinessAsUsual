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

# Restore
RUN dotnet restore frontend/BusinessAsUsual.Web/BusinessAsUsual.Web.csproj

# Copy everything
COPY frontend/BusinessAsUsual.Web frontend/BusinessAsUsual.Web
COPY backend/BusinessAsUsual.Domain backend/BusinessAsUsual.Domain
COPY backend/BusinessAsUsual.Application backend/BusinessAsUsual.Application
COPY backend/BusinessAsUsual.Infrastructure backend/BusinessAsUsual.Infrastructure

# Publish
RUN dotnet publish frontend/BusinessAsUsual.Web/BusinessAsUsual.Web.csproj -c Release -o /app/publish

# ============================
# RUNTIME STAGE
# ============================
ENV ASPNETCORE_URLS=http://+:80
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BusinessAsUsual.Web.dll"]