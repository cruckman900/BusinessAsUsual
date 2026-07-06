# ============================
# BUILD STAGE
# ============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# AI.Api is self-contained (no project references), so only its csproj is needed to restore
COPY services/AI/AI.Api/AI.Api.csproj services/AI/AI.Api/

# Restore
RUN dotnet restore services/AI/AI.Api/AI.Api.csproj

# Copy source
COPY services/AI/AI.Api services/AI/AI.Api

# Publish
RUN dotnet publish services/AI/AI.Api/AI.Api.csproj -c Release -o /app/publish

# ============================
# RUNTIME STAGE
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AI.Api.dll"]
