# ============================
# BUILD STAGE
# ============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files (respecting the Sales dependency graph) for a cached restore
COPY services/Sales/Sales.Web/Sales.Web.csproj services/Sales/Sales.Web/
COPY services/Sales/Sales.Application/Sales.Application.csproj services/Sales/Sales.Application/
COPY services/Sales/Sales.Domain/Sales.Domain.csproj services/Sales/Sales.Domain/
COPY services/Sales/Sales.Infrastructure/Sales.Infrastructure.csproj services/Sales/Sales.Infrastructure/

# Restore
RUN dotnet restore services/Sales/Sales.Web/Sales.Web.csproj

# Copy source
COPY services/Sales/Sales.Web services/Sales/Sales.Web
COPY services/Sales/Sales.Application services/Sales/Sales.Application
COPY services/Sales/Sales.Domain services/Sales/Sales.Domain
COPY services/Sales/Sales.Infrastructure services/Sales/Sales.Infrastructure

# Publish
RUN dotnet publish services/Sales/Sales.Web/Sales.Web.csproj -c Release -o /app/publish

# ============================
# RUNTIME STAGE
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Sales.Web.dll"]
