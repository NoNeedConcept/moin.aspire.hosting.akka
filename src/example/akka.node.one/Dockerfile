FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["example/akka.node.one/akka.node.one.csproj", "example/akka.node.one/"]
COPY ["example/akka.core/akka.core.csproj", "example/akka.core/"]
COPY ["hosting/moin.akka.cluster.hosting.aspire/moin.akka.cluster.hosting.aspire.csproj", "hosting/moin.akka.cluster.hosting.aspire/"]
COPY ["hosting/moin.akka.remote.hosting.aspire/moin.akka.remote.hosting.aspire.csproj", "hosting/moin.akka.remote.hosting.aspire/"]
RUN dotnet restore "example/akka.node.one/akka.node.one.csproj"
COPY . .
WORKDIR "/src/example/akka.node.one"
RUN dotnet build "./akka.node.one.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./akka.node.one.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "akka.node.one.dll"]
