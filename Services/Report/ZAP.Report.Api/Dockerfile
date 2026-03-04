FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy all projects
COPY ["ZAP.sln", "./"]
COPY ["Shared/BuildingBlocks/ZAP.BuildingBlocks/ZAP.BuildingBlocks.csproj", "Shared/BuildingBlocks/ZAP.BuildingBlocks/"]
COPY ["Services/Report/ZAP.Report.Api/ZAP.Report.Api.csproj", "Services/Report/ZAP.Report.Api/"]
COPY ["Services/Report/ZAP.Report.Application/ZAP.Report.Application.csproj", "Services/Report/ZAP.Report.Application/"]
COPY ["Services/Report/ZAP.Report.Domain/ZAP.Report.Domain.csproj", "Services/Report/ZAP.Report.Domain/"]
COPY ["Services/Report/ZAP.Report.Infrastructure/ZAP.Report.Infrastructure.csproj", "Services/Report/ZAP.Report.Infrastructure/"]

# Restore
RUN dotnet restore "Services/Report/ZAP.Report.Api/ZAP.Report.Api.csproj"

# Copy everything
COPY . .

# Build
WORKDIR "/src/Services/Report/ZAP.Report.Api"
RUN dotnet build "ZAP.Report.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ZAP.Report.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZAP.Report.Api.dll"]
