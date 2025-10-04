# Use official .NET 9 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["BusinessAsUsual/business_as_usual.csproj", "BusinessAsUsual/"]
RUN dotnet restore "BusinessAsUsual/business_as_usual.csproj"
COPY . .
WORKDIR "/src/BusinessAsUsual"
RUN dotnet build -c Release -o /app/build
RUN dotnet publish -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "business_as_usual.dll"]