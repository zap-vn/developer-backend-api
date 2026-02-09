# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["Zap.Backend.sln", "./"]
COPY ["gateway/Zap.Gateway.Api/Zap.Gateway.Api.csproj", "gateway/Zap.Gateway.Api/"]
COPY ["shared/Zap.BuildingBlocks/Zap.BuildingBlocks.csproj", "shared/Zap.BuildingBlocks/"]
COPY ["shared/Zap.Contracts/Zap.Contracts.csproj", "shared/Zap.Contracts/"]

# Restore dependencies
RUN dotnet restore "gateway/Zap.Gateway.Api/Zap.Gateway.Api.csproj"

# Copy everything else
COPY . .

# Build the application
WORKDIR "/src/gateway/Zap.Gateway.Api"
RUN dotnet build "Zap.Gateway.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Zap.Gateway.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health check
USER root
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Create non-root user for security
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# Copy published application
COPY --from=publish /app/publish .

# Configure environment
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl --fail http://localhost:8080/health || exit 1

# Expose port
EXPOSE 8080

# Entry point
ENTRYPOINT ["dotnet", "Zap.Gateway.Api.dll"]
