# ============================
# BUILD STAGE
# ============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files (respecting the Services.Web dependency graph) for a cached restore
COPY services/Services/Services.Web/Services.Web.csproj services/Services/Services.Web/
COPY services/Services/Services.Application/Services.Application.csproj services/Services/Services.Application/
COPY services/Services/Services.Infrastructure/Services.Infrastructure.csproj services/Services/Services.Infrastructure/
COPY services/Services/Services.Domain/Services.Domain.csproj services/Services/Services.Domain/
COPY services/Platform/Platform.Web/Platform.Web.csproj services/Platform/Platform.Web/
COPY services/Platform/Platform.Application/Platform.Application.csproj services/Platform/Platform.Application/
COPY services/Platform/Platform.Infrastructure/Platform.Infrastructure.csproj services/Platform/Platform.Infrastructure/
COPY services/Platform/Platform.Domain/Platform.Domain.csproj services/Platform/Platform.Domain/
COPY shared/BusinessAsUsual.Shared.UI/BusinessAsUsual.Shared.UI.csproj shared/BusinessAsUsual.Shared.UI/
COPY BusinessAsUsual.Core/BusinessAsUsual.Core.csproj BusinessAsUsual.Core/

# Restore
RUN dotnet restore services/Services/Services.Web/Services.Web.csproj

# Copy source
COPY services/Services/Services.Web services/Services/Services.Web
COPY services/Services/Services.Application services/Services/Services.Application
COPY services/Services/Services.Infrastructure services/Services/Services.Infrastructure
COPY services/Services/Services.Domain services/Services/Services.Domain
COPY services/Platform/Platform.Web services/Platform/Platform.Web
COPY services/Platform/Platform.Application services/Platform/Platform.Application
COPY services/Platform/Platform.Infrastructure services/Platform/Platform.Infrastructure
COPY services/Platform/Platform.Domain services/Platform/Platform.Domain
COPY shared/BusinessAsUsual.Shared.UI shared/BusinessAsUsual.Shared.UI
COPY BusinessAsUsual.Core BusinessAsUsual.Core

# Publish
RUN dotnet publish services/Services/Services.Web/Services.Web.csproj -c Release -o /app/publish

# ============================
# RUNTIME STAGE
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Services.Web.dll"]
