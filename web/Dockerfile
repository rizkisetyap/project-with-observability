FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# COPY ../Observability ./Observability
COPY ../ ./
RUN dotnet restore web/web.csproj


RUN dotnet publish web/web.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app
COPY --from=build /app/publish .


EXPOSE 8080
# Add HEALTHCHECK
HEALTHCHECK --interval=30s --timeout=10s --start-period=10s --retries=3 \
  CMD curl -f http://localhost:8080/health/live || exit 1
ENTRYPOINT [ "dotnet","web.dll" ]