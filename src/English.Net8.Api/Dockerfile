FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["English.Net8.Api/English.Net8.Api.csproj", "dependencies/"]
RUN dotnet restore "dependencies/English.Net8.Api.csproj"
COPY . .
WORKDIR English.Net8.Api
RUN dotnet build -c release -o /app/build

FROM build AS publish
RUN dotnet publish -c release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "English.Net8.Api.dll"]

