FROM mcr.microsoft.com/dotnet/sdk:9.0
WORKDIR /app
COPY BusinessAsUsual.Tests/*.csproj ./BusinessAsUsual.Tests/
COPY BusinessAsUsual.Admin/*.csproj ./BusinessAsUsual.Admin/
RUN dotnet restore
COPY . .
RUN dotnet test --no-build --verbosity normal