# ============================
# BUILD STAGE
# ============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files (respecting the Finance dependency graph) for a cached restore
COPY services/Finance/Finance.API/Finance.API.csproj services/Finance/Finance.API/
COPY services/Finance/Finance.Application/Finance.Application.csproj services/Finance/Finance.Application/
COPY services/Finance/Finance.Contracts/Finance.Contracts.csproj services/Finance/Finance.Contracts/
COPY services/Finance/Finance.Domain/Finance.Domain.csproj services/Finance/Finance.Domain/
COPY BusinessAsUsual.Core/BusinessAsUsual.Core.csproj BusinessAsUsual.Core/

# Restore
# Retry the isolated restore to survive transient NuGet connectivity on small
# EC2 instances (a flaky network otherwise fails restore with exit code 82).
RUN for i in 1 2 3; do \
	  dotnet restore services/Finance/Finance.API/Finance.API.csproj && break; \
	  if [ "$i" = "3" ]; then echo "restore failed after 3 attempts"; exit 1; fi; \
	  echo "restore attempt $i failed; retrying in 10s..."; sleep 10; \
	done

# Copy source
COPY services/Finance/Finance.API services/Finance/Finance.API
COPY services/Finance/Finance.Application services/Finance/Finance.Application
COPY services/Finance/Finance.Contracts services/Finance/Finance.Contracts
COPY services/Finance/Finance.Domain services/Finance/Finance.Domain
COPY BusinessAsUsual.Core BusinessAsUsual.Core

# Publish
RUN dotnet publish services/Finance/Finance.API/Finance.API.csproj -c Release -o /app/publish

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
ENTRYPOINT ["dotnet", "Finance.API.dll"]
