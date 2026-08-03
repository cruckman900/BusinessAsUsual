# ============================
# BUILD STAGE
# ============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files (respecting the Sales dependency graph) for a cached restore
COPY services/Sales/Sales.API/Sales.API.csproj services/Sales/Sales.API/
COPY services/Sales/Sales.Application/Sales.Application.csproj services/Sales/Sales.Application/
COPY services/Sales/Sales.Contracts/Sales.Contracts.csproj services/Sales/Sales.Contracts/
COPY services/Sales/Sales.Domain/Sales.Domain.csproj services/Sales/Sales.Domain/
COPY services/Sales/Sales.Infrastructure/Sales.Infrastructure.csproj services/Sales/Sales.Infrastructure/
COPY BusinessAsUsual.Core/BusinessAsUsual.Core.csproj BusinessAsUsual.Core/

# Restore
# Retry the isolated restore to survive transient NuGet connectivity on small
# EC2 instances (a flaky network otherwise fails restore with exit code 82).
RUN for i in 1 2 3; do \
	  dotnet restore services/Sales/Sales.API/Sales.API.csproj && break; \
	  if [ "$i" = "3" ]; then echo "restore failed after 3 attempts"; exit 1; fi; \
	  echo "restore attempt $i failed; retrying in 10s..."; sleep 10; \
	done

# Copy source
COPY services/Sales/Sales.API services/Sales/Sales.API
COPY services/Sales/Sales.Application services/Sales/Sales.Application
COPY services/Sales/Sales.Contracts services/Sales/Sales.Contracts
COPY services/Sales/Sales.Domain services/Sales/Sales.Domain
COPY services/Sales/Sales.Infrastructure services/Sales/Sales.Infrastructure
COPY BusinessAsUsual.Core BusinessAsUsual.Core

# Publish
RUN dotnet publish services/Sales/Sales.API/Sales.API.csproj -c Release -o /app/publish

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
ENTRYPOINT ["dotnet", "Sales.API.dll"]
