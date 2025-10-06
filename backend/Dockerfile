# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy all project files
COPY ["backend/BusinessAsUsual.API/BusinessAsUsual.API.csproj", "backend/BusinessAsUsual.API/"]
COPY ["backend/BusinessAsUsual.Application/BusinessAsUsual.Application.csproj", "backend/BusinessAsUsual.Application/"]
COPY ["backend/BusinessAsUsual.Domain/BusinessAsUsual.Domain.csproj", "backend/BusinessAsUsual.Domain/"]
COPY ["backend/BusinessAsUsual.Analyzers/BusinessAsUsual.Analyzers.csproj", "backend/BusinessAsUsual.Analyzers/"]

# Restore dependencies
RUN dotnet restore "backend/BusinessAsUsual.API/BusinessAsUsual.API.csproj"

# Copy the full source tree
COPY . .

# Publish only the API project
WORKDIR "/src/backend/BusinessAsUsual.API"
RUN dotnet publish "BusinessAsUsual.API.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "BusinessAsUsual.API.dll"]