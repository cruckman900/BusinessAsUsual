# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files for restore
COPY ["frontend/BusinessAsUsual.Web/BusinessAsUsual.Web.csproj", "BusinessAsUsual.Web/"]
COPY ["../BusinessAsUsual.Infrastructure/BusinessAsUsual.Infrastructure.csproj", "BusinessAsUsual.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "BusinessAsUsual.Web/BusinessAsUsual.Web.csproj"

# Copy full source tree
COPY . .

# Publish the frontend project
WORKDIR "/src/frontend/BusinessAsUsual.Web"
RUN dotnet publish "BusinessAsUsual.Web.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "BusinessAsUsual.Web.dll"]