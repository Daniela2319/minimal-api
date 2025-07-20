FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY minimal-api.csproj ./
COPY . ./
RUN dotnet restore "./minimal-api.csproj"
RUN dotnet publish "./minimal-api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "minimal-api.dll"]