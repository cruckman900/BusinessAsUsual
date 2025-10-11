FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy test and admin projects
COPY ["BusinessAsUsual.Tests/BusinessAsUsual.Tests.csproj", "BusinessAsUsual.Tests/"]
COPY ["BusinessAsUsual.Admin/BusinessAsUsual.Admin.csproj", "BusinessAsUsual.Admin/"]
COPY ["BusinessAsUsual.Infrastructure/BusinessAsUsual.Infrastructure.csproj", "BusinessAsUsual.Infrastructure/"]

# Restore test project
COPY ["BusinessAsUsual.sln", "."]
RUN dotnet restore "BusinessAsUsual.sln"

# Copy everything
COPY . .

# Run tests
WORKDIR /app/BusinessAsUsual.Tests
RUN dotnet test --verbosity normal