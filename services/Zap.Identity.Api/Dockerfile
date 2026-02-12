# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["services/Zap.Identity.Api/Zap.Identity.Api.csproj", "services/Zap.Identity.Api/"]
COPY ["services/Zap.Identity.Application/Zap.Identity.Application.csproj", "services/Zap.Identity.Application/"]
COPY ["services/Zap.Identity.Domain/Zap.Identity.Domain.csproj", "services/Zap.Identity.Domain/"]
COPY ["services/Zap.Identity.Infrastructure/Zap.Identity.Infrastructure.csproj", "services/Zap.Identity.Infrastructure/"]

RUN dotnet restore "services/Zap.Identity.Api/Zap.Identity.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/services/Zap.Identity.Api"
RUN dotnet build "Zap.Identity.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Zap.Identity.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Zap.Identity.Api.dll"]
