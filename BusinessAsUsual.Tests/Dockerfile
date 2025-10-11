FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy test and admin projects
COPY ["BusinessAsUsual.Tests/BusinessAsUsual.Tests.csproj", "BusinessAsUsual.Tests/"]
COPY ["BusinessAsUsual.Admin/BusinessAsUsual.Admin.csproj", "BusinessAsUsual.Admin/"]

# Restore test project
RUN dotnet restore "BusinessAsUsual.Tests/BusinessAsUsual.Tests.csproj"

# Copy everything
COPY . .

# Run tests
WORKDIR /app/BusinessAsUsual.Tests
RUN dotnet test --verbosity normal