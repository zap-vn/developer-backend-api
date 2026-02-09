# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["Zap.Backend.sln", "./"]
COPY ["services/Zap.Identity.Api/Zap.Identity.Api.csproj", "services/Zap.Identity.Api/"]
COPY ["services/Zap.Identity.Application/Zap.Identity.Application.csproj", "services/Zap.Identity.Application/"]
COPY ["services/Zap.Identity.Domain/Zap.Identity.Domain.csproj", "services/Zap.Identity.Domain/"]
COPY ["services/Zap.Identity.Infrastructure/Zap.Identity.Infrastructure.csproj", "services/Zap.Identity.Infrastructure/"]
COPY ["shared/Zap.BuildingBlocks/Zap.BuildingBlocks.csproj", "shared/Zap.BuildingBlocks/"]
COPY ["shared/Zap.Contracts/Zap.Contracts.csproj", "shared/Zap.Contracts/"]

# Restore dependencies
RUN dotnet restore "services/Zap.Identity.Api/Zap.Identity.Api.csproj"

# Copy everything else
COPY . .

# Build the application
WORKDIR "/src/services/Zap.Identity.Api"
RUN dotnet build "Zap.Identity.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Zap.Identity.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

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
ENTRYPOINT ["dotnet", "Zap.Identity.Api.dll"]
