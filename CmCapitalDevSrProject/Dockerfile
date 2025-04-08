FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ENV ASPNETCORE_URLS=http://+:8080
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY CmCapitalDevSrProject.csproj .
RUN dotnet restore "CmCapitalDevSrProject.csproj"
COPY . .
RUN dotnet build "CmCapitalDevSrProject.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CmCapitalDevSrProject.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "CmCapitalDevSrProject.dll"]
