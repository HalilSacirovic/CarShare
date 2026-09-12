FROM mcr.microsoft.com/dotnet/sdk:8.0.202 AS build

WORKDIR /source

COPY ["src/CarShare.API/CarShare.API.csproj", "src/CarShare.API/"]
COPY ["src/CarShare.Application/CarShare.Application.csproj", "src/CarShare.Application/"]
COPY ["src/CarShare.Domain/CarShare.Domain.csproj", "src/CarShare.Domain/"]
COPY ["src/CarShare.Infrastructure/CarShare.Infrastructure.csproj", "src/CarShare.Infrastructure/"]

RUN dotnet restore "src/CarShare.API/CarShare.API.csproj"

COPY . .

RUN dotnet publish "src/CarShare.API/CarShare.API.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

RUN mkdir -p /app/wwwroot/uploads

EXPOSE 8080

ENTRYPOINT ["sh", "-c", "exec dotnet CarShare.API.dll --urls http://0.0.0.0:${PORT:-8080}"]