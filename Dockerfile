# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy everything
COPY . .

# Restore using the solution file
RUN dotnet restore BusinessAsUsual.sln

# Publish the frontend project
WORKDIR /src/frontend/BusinessAsUsual.Web
RUN dotnet publish "BusinessAsUsual.Web.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "BusinessAsUsual.Web.dll"]