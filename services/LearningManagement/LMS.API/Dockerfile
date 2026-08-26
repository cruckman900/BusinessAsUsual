# ============================
# BUILD STAGE
# ============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files (respecting the LMS dependency graph) for a cached restore
COPY services/LearningManagement/LMS.API/LMS.API.csproj services/LearningManagement/LMS.API/
COPY services/LearningManagement/LMS.Application/LMS.Application.csproj services/LearningManagement/LMS.Application/
COPY services/LearningManagement/LMS.Infrastructure/LMS.Infrastructure.csproj services/LearningManagement/LMS.Infrastructure/
COPY services/LearningManagement/LMS.Contracts/LMS.Contracts.csproj services/LearningManagement/LMS.Contracts/
COPY services/LearningManagement/LMS.Domain/LMS.Domain.csproj services/LearningManagement/LMS.Domain/
COPY BusinessAsUsual.Core/BusinessAsUsual.Core.csproj BusinessAsUsual.Core/

# Restore
# Retry the isolated restore to survive transient NuGet connectivity on small
# EC2 instances (a flaky network otherwise fails restore with exit code 82).
RUN for i in 1 2 3; do \
	  dotnet restore services/LearningManagement/LMS.API/LMS.API.csproj && break; \
	  if [ "$i" = "3" ]; then echo "restore failed after 3 attempts"; exit 1; fi; \
	  echo "restore attempt $i failed; retrying in 10s..."; sleep 10; \
	done

# Copy source
COPY services/LearningManagement/LMS.API services/LearningManagement/LMS.API
COPY services/LearningManagement/LMS.Application services/LearningManagement/LMS.Application
COPY services/LearningManagement/LMS.Infrastructure services/LearningManagement/LMS.Infrastructure
COPY services/LearningManagement/LMS.Contracts services/LearningManagement/LMS.Contracts
COPY services/LearningManagement/LMS.Domain services/LearningManagement/LMS.Domain
COPY BusinessAsUsual.Core BusinessAsUsual.Core

# Publish
RUN dotnet publish services/LearningManagement/LMS.API/LMS.API.csproj -c Release -o /app/publish

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
ENTRYPOINT ["dotnet", "LMS.API.dll"]
