# AvanadeStore.Auth
# BASE
FROM mcr.microsoft.com/dotnet/aspnet:9.0-bookworm-slim AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# BUILD
FROM mcr.microsoft.com/dotnet/sdk:9.0-bookworm-slim AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AvanadeStore.Auth/src/Auth.API/Auth.API.csproj", "AvanadeStore.Auth/src/Auth.API/"]
COPY ["AvanadeStore.Auth/src/Auth.Application/Auth.Application.csproj", "AvanadeStore.Auth/src/Auth.Application/"]
COPY ["AvanadeStore.Auth/src/Auth.Domain/Auth.Domain.csproj", "AvanadeStore.Auth/src/Auth.Domain/"]
COPY ["AvanadeStore.Auth/src/Auth.Exception/Auth.Exception.csproj", "AvanadeStore.Auth/src/Auth.Exception/"]
COPY ["AvanadeStore.Auth/src/Auth.Infra/Auth.Infra.csproj", "AvanadeStore.Auth/src/Auth.Infra/"]
RUN dotnet restore "./AvanadeStore.Auth/src/Auth.API/Auth.API.csproj"
COPY . .
WORKDIR "/src/AvanadeStore.Auth/src/Auth.API"
RUN dotnet build "./Auth.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# PUBLISH
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Auth.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# RUN
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Auth.API.dll"]
