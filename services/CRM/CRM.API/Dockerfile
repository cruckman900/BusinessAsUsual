# ============================
# BUILD STAGE
# ============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files (respecting the CRM dependency graph) for a cached restore
COPY services/CRM/CRM.API/CRM.API.csproj services/CRM/CRM.API/
COPY services/CRM/CRM.Application/CRM.Application.csproj services/CRM/CRM.Application/
COPY services/CRM/CRM.Domain/CRM.Domain.csproj services/CRM/CRM.Domain/

# Restore
RUN dotnet restore services/CRM/CRM.API/CRM.API.csproj

# Copy source
COPY services/CRM/CRM.API services/CRM/CRM.API
COPY services/CRM/CRM.Application services/CRM/CRM.Application
COPY services/CRM/CRM.Domain services/CRM/CRM.Domain

# Publish
RUN dotnet publish services/CRM/CRM.API/CRM.API.csproj -c Release -o /app/publish

# ============================
# RUNTIME STAGE
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CRM.API.dll"]
