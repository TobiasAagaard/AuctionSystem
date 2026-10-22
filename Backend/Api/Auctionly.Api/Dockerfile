FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Backend/Api/Auctionly.Api/Auctionly.Api.csproj", "Backend/Api/Auctionly.Api/"]
COPY ["Backend/Modules/Auctions/Auctionly.Modules.Auctions.Api/Auctionly.Modules.Auctions.Api.csproj", "Backend/Modules/Auctions/Auctionly.Modules.Auctions.Api/"]
RUN dotnet restore "Backend/Api/Auctionly.Api/Auctionly.Api.csproj"
COPY . .
WORKDIR "/src/Backend/Api/Auctionly.Api"
RUN dotnet build "./Auctionly.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Auctionly.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Auctionly.Api.dll"]
