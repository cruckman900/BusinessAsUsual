# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy everything from the solution root
COPY ../ .

# Restore using the solution file
RUN dotnet restore BusinessAsUsual.sln

# Publish the API project
WORKDIR /src/backend/BusinessAsUsual.API
RUN dotnet publish "BusinessAsUsual.API.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "BusinessAsUsual.API.dll"]