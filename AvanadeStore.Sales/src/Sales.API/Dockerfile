# AvanadeStore.Sales
# BASE
FROM mcr.microsoft.com/dotnet/aspnet:9.0-bookworm-slim AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# BUILD
FROM mcr.microsoft.com/dotnet/sdk:9.0-bookworm-slim AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AvanadeStore.Sales/src/Sales.API/Sales.API.csproj", "AvanadeStore.Sales/src/Sales.API/"]
COPY ["AvanadeStore.Sales/src/Sales.Application/Sales.Application.csproj", "AvanadeStore.Sales/src/Sales.Application/"]
COPY ["AvanadeStore.Sales/src/Sales.Domain/Sales.Domain.csproj", "AvanadeStore.Sales/src/Sales.Domain/"]
COPY ["AvanadeStore.Sales/src/Sales.Exception/Sales.Exception.csproj", "AvanadeStore.Sales/src/Sales.Exception/"]
COPY ["AvanadeStore.Sales/src/Sales.Infra/Sales.Infra.csproj", "AvanadeStore.Sales/src/Sales.Infra/"]
RUN dotnet restore "./AvanadeStore.Sales/src/Sales.API/Sales.API.csproj"
COPY . .
WORKDIR "/src/AvanadeStore.Sales/src/Sales.API"
RUN dotnet build "./Sales.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# PUBLISH
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Sales.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# RUN
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sales.API.dll"]
