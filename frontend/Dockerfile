FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# COPY ../Observability ./Observability
COPY ../ ./
RUN dotnet restore frontend/frontend.csproj


RUN dotnet publish frontend/frontend.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:5000
EXPOSE 5000

# Add HEALTHCHECK
HEALTHCHECK --interval=30s --timeout=10s --start-period=10s --retries=3 \
  CMD curl -f http://localhost:5000/health/live || exit 1
ENTRYPOINT [ "dotnet","frontend.dll" ]