FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /app

# Copy everything into the container
COPY . .

# Restore dependencies
RUN dotnet restore BusinessAsUsual.sln

# Build the solution
RUN dotnet build BusinessAsUsual.sln --no-restore

# Run tests (optional: target specific test project)
RUN dotnet test BusinessAsUsual.Tests/BusinessAsUsual.Tests.csproj --no-build --settings TestConfig.runsettings