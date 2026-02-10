# Basisimage für Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build-Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy both projects
COPY ./CacxServer ./CacxServer
COPY ./CacxShared ./CacxShared

# Restore
WORKDIR /src/CacxServer
RUN dotnet restore "CacxServer.csproj"

# Build
RUN dotnet build "CacxServer.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
RUN dotnet publish "CacxServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final-Stage
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CacxServer.dll"]

