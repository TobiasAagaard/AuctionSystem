FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Api/AuctionSystem.Api/AuctionSystem.Api.csproj", "src/Api/AuctionSystem.Api/"]
RUN dotnet restore "src/Api/AuctionSystem.Api/AuctionSystem.Api.csproj"
COPY . .
WORKDIR "/src/src/Api/AuctionSystem.Api"
RUN dotnet build "./AuctionSystem.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AuctionSystem.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuctionSystem.Api.dll"]
