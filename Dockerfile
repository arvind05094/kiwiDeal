FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /repo

COPY kiwiDeal.slnx ./
COPY Directory.Build.props ./
COPY src/ ./src/

RUN dotnet publish src/kiwiDeal.Api/kiwiDeal.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-self-contained

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "kiwiDeal.Api.dll"]
