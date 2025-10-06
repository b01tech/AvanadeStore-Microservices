# AvanadeStore.Inventory
# BASE
FROM mcr.microsoft.com/dotnet/aspnet:9.0-bookworm-slim AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# BUILD
FROM mcr.microsoft.com/dotnet/sdk:9.0-bookworm-slim AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AvanadeStore.Inventory/src/Inventory.API/Inventory.API.csproj", "AvanadeStore.Inventory/src/Inventory.API/"]
COPY ["AvanadeStore.Inventory/src/Inventory.Application/Inventory.Application.csproj", "AvanadeStore.Inventory/src/Inventory.Application/"]
COPY ["AvanadeStore.Inventory/src/Inventory.Domain/Inventory.Domain.csproj", "AvanadeStore.Inventory/src/Inventory.Domain/"]
COPY ["AvanadeStore.Inventory/src/Inventory.Exception/Inventory.Exception.csproj", "AvanadeStore.Inventory/src/Inventory.Exception/"]
COPY ["AvanadeStore.Inventory/src/Inventory.Infra/Inventory.Infra.csproj", "AvanadeStore.Inventory/src/Inventory.Infra/"]
RUN dotnet restore "./AvanadeStore.Inventory/src/Inventory.API/Inventory.API.csproj"
COPY . .
WORKDIR "/src/AvanadeStore.Inventory/src/Inventory.API"
RUN dotnet build "./Inventory.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# PUBLISH
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Inventory.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# RUN
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Inventory.API.dll"]
