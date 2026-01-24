# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS admin_build
WORKDIR /src

# Copy ONLY the Admin project file + referenced project files
COPY BusinessAsUsual.Admin/BusinessAsUsual.Admin.csproj BusinessAsUsual.Admin/
COPY BusinessAsUsual.Core/BusinessAsUsual.Core.csproj BusinessAsUsual.Core/
COPY BusinessAsUsual.Infrastructure/BusinessAsUsual.Infrastructure.csproj BusinessAsUsual.Infrastructure/
COPY backend/BusinessAsUsual.Application/BusinessAsUsual.Application.csproj backend/BusinessAsUsual.Application/
COPY backend/BusinessAsUsual.Domain/BusinessAsUsual.Domain.csproj backend/BusinessAsUsual.Domain/

# Restore Admin (this restores all referenced projects too)
RUN dotnet restore BusinessAsUsual.Admin/BusinessAsUsual.Admin.csproj

# Copy full source
COPY . .

# Build and publish Admin
WORKDIR /src/BusinessAsUsual.Admin
RUN dotnet publish -c Release -o /app/publish

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=admin_build /app/publish .
ENTRYPOINT ["dotnet", "BusinessAsUsual.Admin.dll"]