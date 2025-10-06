# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all project files
COPY ["BusinessAsUsual.API/BusinessAsUsual.API.csproj", "BusinessAsUsual.API/"]
COPY ["BusinessAsUsual.Application/BusinessAsUsual.Application.csproj", "BusinessAsUsual.Application/"]
COPY ["BusinessAsUsual.Domain/BusinessAsUsual.Domain.csproj", "BusinessAsUsual.Domain/"]
COPY ["BusinessAsUsual.Analyzers/BusinessAsUsual.Analyzers.csproj", "BusinessAsUsual.Analyzers/"]

# Restore dependencies
RUN dotnet restore "BusinessAsUsual.API/BusinessAsUsual.API.csproj"

# Copy the full source tree
COPY . .

# Publish only the API project
WORKDIR /src/BusinessAsUsual.API
RUN dotnet publish "BusinessAsUsual.API.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "BusinessAsUsual.API.dll"]