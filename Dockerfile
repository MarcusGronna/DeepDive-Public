FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY InfraSprint.Api/InfraSprint.Api.csproj InfraSprint.Api/
RUN dotnet restore InfraSprint.Api/InfraSprint.Api.csproj

COPY . .
RUN dotnet publish InfraSprint.Api/InfraSprint.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

RUN mkdir -p /app/Data

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "InfraSprint.Api.dll"]
