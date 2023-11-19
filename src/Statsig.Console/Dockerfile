FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Statsig.Console/Statsig.Console.csproj", "src/Statsig.Console/"]
RUN dotnet restore "src/Statsig.Console/Statsig.Console.csproj"
COPY . .
WORKDIR "/src/src/Statsig.Console"
RUN dotnet build "Statsig.Console.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Statsig.Console.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Statsig.Console.dll"]
