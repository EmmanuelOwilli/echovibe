# Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish EchoVibe.csproj -c Release -o /app/publish /p:UseAppHost=false

# Run
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

# Ensure upload folders exist and are writable
RUN mkdir -p /app/wwwroot/uploads/vibes /app/wwwroot/uploads/profiles \
    && chmod -R 777 /app/wwwroot/uploads

ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["sh", "-c", "dotnet EchoVibe.dll --urls http://0.0.0.0:${PORT:-8080}"]