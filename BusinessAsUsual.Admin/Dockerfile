# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy only project files for restore caching
COPY BusinessAsUsual.Admin/BusinessAsUsual.Admin.csproj BusinessAsUsual.Admin/
COPY BusinessAsUsual.Infrastructure/BusinessAsUsual.Infrastructure.csproj BusinessAsUsual.Infrastructure/
# COPY BusinessAsUsual.Core/BusinessAsUsual.Core.csproj BusinessAsUsual.Core/
# COPY BusinessAsUsual.Shared/BusinessAsUsual.Shared.csproj BusinessAsUsual.Shared/

# Restore dependencies
RUN dotnet restore BusinessAsUsual.Admin/BusinessAsUsual.Admin.csproj

# Copy full source tree
COPY . .
COPY .env.ci /app/.env
# Optional: validate env vars (SmartEnvValidator hook)
# RUN ./scripts/validate-env.sh || echo "Skipping env validation in build stage"

# Build and publish
WORKDIR /src/BusinessAsUsual.Admin
RUN dotnet publish -c Release -o /app/publish

# Final runtime image
FROM base AS final
WORKDIR /app

# Copy published output
COPY --from=build /app/publish .
COPY --from=build /app/.env /app/.env

# Optional: validate env vars again at runtime
# COPY ./scripts/validate-env.sh .
# RUN chmod +x validate-env.sh
# ENTRYPOINT ["./validate-env.sh", "&&", "dotnet", "BusinessAsUsual.Admin.dll"]

ENTRYPOINT ["dotnet", "BusinessAsUsual.Admin.dll"]