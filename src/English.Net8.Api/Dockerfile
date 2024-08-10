FROM mcr.microsoft.com/dotnet/aspnet:8.0 as base
USER app 
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY *.csproj ./aspnetapp/
RUN dotnet restore ./aspnetapp/*.csproj
COPY . ./aspnetapp/
WORKDIR ./aspnetapp/
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base as final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "English.Net8.Api.dll"]

