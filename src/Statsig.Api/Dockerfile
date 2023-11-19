FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Statsig.Api/Statsig.Api.csproj", "src/Statsig.Api/"]
RUN dotnet restore "src/Statsig.Api/Statsig.Api.csproj"
COPY . .
WORKDIR "/src/src/Statsig.Api"
RUN dotnet build "Statsig.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Statsig.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Statsig.Api.dll"]
