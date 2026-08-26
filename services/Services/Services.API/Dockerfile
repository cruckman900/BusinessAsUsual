# ============================
# BUILD STAGE
# ============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files (respecting the Services dependency graph) for a cached restore
COPY services/Services/Services.API/Services.API.csproj services/Services/Services.API/
COPY services/Services/Services.Application/Services.Application.csproj services/Services/Services.Application/
COPY services/Services/Services.Infrastructure/Services.Infrastructure.csproj services/Services/Services.Infrastructure/
COPY services/Services/Services.Contracts/Services.Contracts.csproj services/Services/Services.Contracts/
COPY services/Services/Services.Domain/Services.Domain.csproj services/Services/Services.Domain/
COPY BusinessAsUsual.Core/BusinessAsUsual.Core.csproj BusinessAsUsual.Core/

# Restore
# Retry the isolated restore to survive transient NuGet connectivity on small
# EC2 instances (a flaky network otherwise fails restore with exit code 82).
RUN for i in 1 2 3; do \
	  dotnet restore services/Services/Services.API/Services.API.csproj && break; \
	  if [ "$i" = "3" ]; then echo "restore failed after 3 attempts"; exit 1; fi; \
	  echo "restore attempt $i failed; retrying in 10s..."; sleep 10; \
	done

# Copy source
COPY services/Services/Services.API services/Services/Services.API
COPY services/Services/Services.Application services/Services/Services.Application
COPY services/Services/Services.Infrastructure services/Services/Services.Infrastructure
COPY services/Services/Services.Contracts services/Services/Services.Contracts
COPY services/Services/Services.Domain services/Services/Services.Domain
COPY BusinessAsUsual.Core BusinessAsUsual.Core

# Publish
RUN dotnet publish services/Services/Services.API/Services.API.csproj -c Release -o /app/publish

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
ENTRYPOINT ["dotnet", "Services.API.dll"]
