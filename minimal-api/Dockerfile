FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
COPY ./publish .
ENTRYPOINT ["dotnet", "minimal-api.dll"]
