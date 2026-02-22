FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
ARG APP_UID=1000
ARG APP_GID=1000
WORKDIR /app
# Ensure uploads directory exists and is writable by the runtime user
RUN mkdir -p /app/wwwroot/uploads/plants \
 && chown -R ${APP_UID}:${APP_GID} /app/wwwroot || true \
 && chmod -R 0755 /app/wwwroot || true

USER ${APP_UID}
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/GrowMate.Presentation/GrowMate.Presentation.csproj", "src/GrowMate.Presentation/"]
RUN dotnet restore "src/GrowMate.Presentation/GrowMate.Presentation.csproj"
COPY . .
WORKDIR "/src/src/GrowMate.Presentation"
RUN dotnet build "./GrowMate.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GrowMate.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GrowMate.Presentation.dll"]
