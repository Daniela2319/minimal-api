FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
COPY ./publish .
ENTRYPOINT ["dotnet", "minimal-api.dll"]

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["minimal-api/minimal-api.csproj", "minimal-api/"]
RUN dotnet restore "minimal-api/minimal-api.csproj"
COPY . .
WORKDIR "/src/minimal-api"
RUN dotnet build "minimal-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "minimal-api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "minimal-api.dll"]
