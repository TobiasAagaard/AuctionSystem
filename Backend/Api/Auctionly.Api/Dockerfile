FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Backend/Directory.Build.props", "Backend/"]
COPY ["Backend/Api/Auctionly.Api/Auctionly.Api.csproj", "Backend/Api/Auctionly.Api/"]
COPY ["Backend/Modules/Auctions/Auctionly.Modules.Auctions.Domain/Auctionly.Modules.Auctions.Domain.csproj", "Backend/Modules/Auctions/Auctionly.Modules.Auctions.Domain/"]
COPY ["Backend/Modules/Auctions/Auctionly.Modules.Auctions.Application/Auctionly.Modules.Auctions.Application.csproj", "Backend/Modules/Auctions/Auctionly.Modules.Auctions.Application/"]
COPY ["Backend/Modules/Auctions/Auctionly.Modules.Auctions.Presentation/Auctionly.Modules.Auctions.Presentation.csproj", "Backend/Modules/Auctions/Auctionly.Modules.Auctions.Presentation/"]
COPY ["Backend/Modules/Auctions/Auctionly.Modules.Auctions.Infrastructure/Auctionly.Modules.Auctions.Infrastructure.csproj", "Backend/Modules/Auctions/Auctionly.Modules.Auctions.Infrastructure/"]
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
