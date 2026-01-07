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

# Remove GitInfo from global NuGet cache
RUN rm -rf /root/.nuget/packages/gitinfo
RUN rm -rf /root/.nuget/packages/gitinfo*
RUN rm -rf /usr/share/dotnet/sdk/*/Sdks/*/analyzers/*/GitInfo*
RUN rm -rf /usr/share/dotnet/sdk/*/Roslyn/bincore/GitInfo*
# RUN find /root/.nuget/packages -maxdepth 2 -type d -name "gitinfo" -exec rm -rf {} +

# Remove GitInfo from the project
# RUN dotnet remove BusinessAsUsual.Admin/BusinessAsUsual.Admin.csproj package GitInfo

# Restore dependencies
RUN dotnet restore BusinessAsUsual.Admin/BusinessAsUsual.Admin.csproj

# Copy full source tree
COPY . .
COPY .env /app/.env
# Optional: validate env vars (SmartEnvValidator hook)
# RUN ./scripts/validate-env.sh || echo "Skipping env validation in build stage"

ENV GitInfoSkip=true

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