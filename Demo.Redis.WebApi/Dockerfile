FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Demo.Redis.WebApi/Demo.Redis.WebApi.csproj", "Demo.Redis.WebApi/"]
COPY ["Demo.Redis.Infrastructure.Redis/Demo.Redis.Infrastructure.Redis.csproj", "Demo.Redis.Infrastructure.Redis/"]
COPY ["Demo.Redis.Infrastructure.Postgres/Demo.Redis.Infrastructure.Postgres.csproj", "Demo.Redis.Infrastructure.Postgres/"]
RUN dotnet restore "Demo.Redis.WebApi/Demo.Redis.WebApi.csproj"
COPY . .
WORKDIR "/src/Demo.Redis.WebApi"
RUN dotnet build "Demo.Redis.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Demo.Redis.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Demo.Redis.WebApi.dll"]
