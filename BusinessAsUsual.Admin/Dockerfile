FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app
COPY ["BusinessAsUsual.Admin/BusinessAsUsual.Admin.csproj", "BusinessAsUsual.Admin/"]
RUN dotnet restore "BusinessAsUsual.Admin/BusinessAsUsual.Admin.csproj"
COPY . .
WORKDIR /app/BusinessAsUsual.Admin
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BusinessAsUsual.Admin.dll"]