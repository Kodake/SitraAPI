FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SITRA.API/SITRA.API.csproj", "SITRA.API/"]
COPY ["SITRA.Core/SITRA.Core.csproj", "SITRA.Core/"]
COPY ["SITRA.Infrastructure/SITRA.Infrastructure.csproj", "SITRA.Infrastructure/"]
COPY ["SITRA.Tests/SITRA.Tests.csproj", "SITRA.Tests/"]
RUN dotnet restore "./SITRA.API/SITRA.API.csproj"
COPY . .
WORKDIR "/src/SITRA.API"
RUN dotnet build "./SITRA.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SITRA.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SITRA.API.dll"]
