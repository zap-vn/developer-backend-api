FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and shared projects
COPY ["CRM.sln", "./"]
COPY ["Shared/BuildingBlocks/CRM.BuildingBlocks/CRM.BuildingBlocks.csproj", "Shared/BuildingBlocks/CRM.BuildingBlocks/"]

# Copy Promotion project files
COPY ["Services/Promotion/CRM.Promotion.Api/CRM.Promotion.Api.csproj", "Services/Promotion/CRM.Promotion.Api/"]
COPY ["Services/Promotion/CRM.Promotion.Application/CRM.Promotion.Application.csproj", "Services/Promotion/CRM.Promotion.Application/"]
COPY ["Services/Promotion/CRM.Promotion.Domain/CRM.Promotion.Domain.csproj", "Services/Promotion/CRM.Promotion.Domain/"]
COPY ["Services/Promotion/CRM.Promotion.Infrastructure/CRM.Promotion.Infrastructure.csproj", "Services/Promotion/CRM.Promotion.Infrastructure/"]

RUN dotnet restore "Services/Promotion/CRM.Promotion.Api/CRM.Promotion.Api.csproj"

# Copy full source
COPY . .

# Build
WORKDIR "/src/Services/Promotion/CRM.Promotion.Api"
RUN dotnet build "CRM.Promotion.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CRM.Promotion.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CRM.Promotion.Api.dll"]
