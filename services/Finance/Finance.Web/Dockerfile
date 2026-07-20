# ============================
# BUILD STAGE
# ============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files (respecting the Finance dependency graph) for a cached restore
COPY services/Finance/Finance.Web/Finance.Web.csproj services/Finance/Finance.Web/
COPY services/Finance/Finance.Application/Finance.Application.csproj services/Finance/Finance.Application/
COPY services/Finance/Finance.Domain/Finance.Domain.csproj services/Finance/Finance.Domain/
COPY BusinessAsUsual.Core/BusinessAsUsual.Core.csproj BusinessAsUsual.Core/

# Restore
RUN dotnet restore services/Finance/Finance.Web/Finance.Web.csproj

# Copy source
COPY services/Finance/Finance.Web services/Finance/Finance.Web
COPY services/Finance/Finance.Application services/Finance/Finance.Application
COPY services/Finance/Finance.Domain services/Finance/Finance.Domain
COPY BusinessAsUsual.Core BusinessAsUsual.Core

# Publish
RUN dotnet publish services/Finance/Finance.Web/Finance.Web.csproj -c Release -o /app/publish

# ============================
# RUNTIME STAGE
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Finance.Web.dll"]
