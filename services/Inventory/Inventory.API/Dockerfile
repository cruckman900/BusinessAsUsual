# ============================
# BUILD STAGE
# ============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files (respecting the Inventory dependency graph) for a cached restore
COPY services/Inventory/Inventory.API/Inventory.API.csproj services/Inventory/Inventory.API/
COPY services/Inventory/Inventory.Application/Inventory.Application.csproj services/Inventory/Inventory.Application/
COPY services/Inventory/Inventory.Contracts/Inventory.Contracts.csproj services/Inventory/Inventory.Contracts/
COPY services/Inventory/Inventory.Domain/Inventory.Domain.csproj services/Inventory/Inventory.Domain/
COPY services/Inventory/Inventory.Infrastructure/Inventory.Infrastructure.csproj services/Inventory/Inventory.Infrastructure/
COPY BusinessAsUsual.Core/BusinessAsUsual.Core.csproj BusinessAsUsual.Core/

# Restore
# Retry the isolated restore to survive transient NuGet connectivity on small
# EC2 instances (a flaky network otherwise fails restore with exit code 82).
RUN for i in 1 2 3; do \
	  dotnet restore services/Inventory/Inventory.API/Inventory.API.csproj && break; \
	  if [ "$i" = "3" ]; then echo "restore failed after 3 attempts"; exit 1; fi; \
	  echo "restore attempt $i failed; retrying in 10s..."; sleep 10; \
	done

# Copy source
COPY services/Inventory/Inventory.API services/Inventory/Inventory.API
COPY services/Inventory/Inventory.Application services/Inventory/Inventory.Application
COPY services/Inventory/Inventory.Contracts services/Inventory/Inventory.Contracts
COPY services/Inventory/Inventory.Domain services/Inventory/Inventory.Domain
COPY services/Inventory/Inventory.Infrastructure services/Inventory/Inventory.Infrastructure
COPY BusinessAsUsual.Core BusinessAsUsual.Core

# Publish
RUN dotnet publish services/Inventory/Inventory.API/Inventory.API.csproj -c Release -o /app/publish

# ============================
# RUNTIME STAGE
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# curl is used by the container HEALTHCHECK below
RUN apt-get update && apt-get install -y --no-install-recommends curl \
	&& rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

HEALTHCHECK --interval=30s --timeout=5s --start-period=20s --retries=3 \
	CMD curl -fsS http://localhost:80/health || exit 1

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Inventory.API.dll"]
