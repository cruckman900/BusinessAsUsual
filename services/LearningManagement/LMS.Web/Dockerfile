# ============================
# BUILD STAGE
# ============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files (respecting the LMS.Web dependency graph) for a cached restore
COPY services/LearningManagement/LMS.Web/LMS.Web.csproj services/LearningManagement/LMS.Web/
COPY services/LearningManagement/LMS.Application/LMS.Application.csproj services/LearningManagement/LMS.Application/
COPY services/LearningManagement/LMS.Infrastructure/LMS.Infrastructure.csproj services/LearningManagement/LMS.Infrastructure/
COPY services/LearningManagement/LMS.Domain/LMS.Domain.csproj services/LearningManagement/LMS.Domain/
COPY services/Platform/Platform.Web/Platform.Web.csproj services/Platform/Platform.Web/
COPY services/Platform/Platform.Application/Platform.Application.csproj services/Platform/Platform.Application/
COPY services/Platform/Platform.Infrastructure/Platform.Infrastructure.csproj services/Platform/Platform.Infrastructure/
COPY services/Platform/Platform.Domain/Platform.Domain.csproj services/Platform/Platform.Domain/
COPY shared/BusinessAsUsual.Shared.UI/BusinessAsUsual.Shared.UI.csproj shared/BusinessAsUsual.Shared.UI/
COPY BusinessAsUsual.Core/BusinessAsUsual.Core.csproj BusinessAsUsual.Core/

# Restore
RUN dotnet restore services/LearningManagement/LMS.Web/LMS.Web.csproj

# Copy source
COPY services/LearningManagement/LMS.Web services/LearningManagement/LMS.Web
COPY services/LearningManagement/LMS.Application services/LearningManagement/LMS.Application
COPY services/LearningManagement/LMS.Infrastructure services/LearningManagement/LMS.Infrastructure
COPY services/LearningManagement/LMS.Domain services/LearningManagement/LMS.Domain
COPY services/Platform/Platform.Web services/Platform/Platform.Web
COPY services/Platform/Platform.Application services/Platform/Platform.Application
COPY services/Platform/Platform.Infrastructure services/Platform/Platform.Infrastructure
COPY services/Platform/Platform.Domain services/Platform/Platform.Domain
COPY shared/BusinessAsUsual.Shared.UI shared/BusinessAsUsual.Shared.UI
COPY BusinessAsUsual.Core BusinessAsUsual.Core

# Publish
RUN dotnet publish services/LearningManagement/LMS.Web/LMS.Web.csproj -c Release -o /app/publish

# ============================
# RUNTIME STAGE
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LMS.Web.dll"]
