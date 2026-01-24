FROM mcr.microsoft.com/dotnet/sdk:9.0 AS backend_build
WORKDIR /src

# Copy project files for restore
COPY backend/BusinessAsUsual.API/BusinessAsUsual.API.csproj backend/BusinessAsUsual.API/
COPY backend/BusinessAsUsual.Domain/BusinessAsUsual.Domain.csproj backend/BusinessAsUsual.Domain/
COPY backend/BusinessAsUsual.Application/BusinessAsUsual.Application.csproj backend/BusinessAsUsual.Application/
COPY BusinessAsUsual.Infrastructure/BusinessAsUsual.Infrastructure.csproj BusinessAsUsual.Infrastructure/

RUN dotnet restore backend/BusinessAsUsual.API/BusinessAsUsual.API.csproj

# Copy full source
COPY backend/BusinessAsUsual.API/ backend/BusinessAsUsual.API/
COPY backend/BusinessAsUsual.Domain/ backend/BusinessAsUsual.Domain/
COPY backend/BusinessAsUsual.Application/ backend/BusinessAsUsual.Application/
COPY BusinessAsUsual.Infrastructure/ BusinessAsUsual.Infrastructure/

WORKDIR /src/backend/BusinessAsUsual.API
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=backend_build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "BusinessAsUsual.API.dll"]