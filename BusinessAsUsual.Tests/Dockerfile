FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy test and admin projects
COPY ["BusinessAsUsual.sln", "."]
COPY ["BusinessAsUsual.Tests/BusinessAsUsual.Tests.csproj", "BusinessAsUsual.Tests/"]
COPY ["BusinessAsUsual.Admin/BusinessAsUsual.Admin.csproj", "BusinessAsUsual.Admin/"]
COPY ["BusinessAsUsual.Infrastructure/BusinessAsUsual.Infrastructure.csproj", "BusinessAsUsual.Infrastructure/"]
COPY ["backend/BusinessAsUsual.API/BusinessAsUsual.API.csproj", "backend/BusinessAsUsual.API/"]
COPY ["backend/BusinessAsUsual.Application/BusinessAsUsual.Application.csproj", "backend/BusinessAsUsual.Application/"]
COPY ["backend/BusinessAsUsual.Domain/BusinessAsUsual.Domain.csproj", "backend/BusinessAsUsual.Domain/"]
COPY ["frontend/BusinessAsUsual.Web/BusinessAsUsual.Web.csproj", "frontend/BusinessAsUsual.Web/"]

# Restore test project
RUN dotnet restore "BusinessAsUsual.sln"

# Copy everything
COPY . .

# Run tests
WORKDIR /app/BusinessAsUsual.Tests
RUN dotnet test --verbosity normal