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
COPY ["src/Sales.API/Sales.API.csproj", "src/Sales.API/"]
COPY ["src/Sales.Application/Sales.Application.csproj", "src/Sales.Application/"]
COPY ["src/Sales.Domain/Sales.Domain.csproj", "src/Sales.Domain/"]
COPY ["src/Sales.Exception/Sales.Exception.csproj", "src/Sales.Exception/"]
COPY ["src/Sales.Infra/Sales.Infra.csproj", "src/Sales.Infra/"]
RUN dotnet restore "./src/Sales.API/Sales.API.csproj"
COPY . .
WORKDIR "/src/src/Sales.API"
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
