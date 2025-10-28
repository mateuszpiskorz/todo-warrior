FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY TodoWarrior.Api/TodoWarrior.Api.csproj TodoWarrior.Api/
RUN dotnet restore TodoWarrior.Api/TodoWarrior.Api.csproj

COPY . .
WORKDIR /src/TodoWarrior.Api
RUN dotnet build TodoWarrior.Api.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish TodoWarrior.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

RUN mkdir -p /data

EXPOSE 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENTRYPOINT ["dotnet", "TodoWarrior.Api.dll"]