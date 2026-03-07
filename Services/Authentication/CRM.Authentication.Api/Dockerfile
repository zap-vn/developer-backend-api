FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Sao chép các tệp project và restore
COPY ["CRM.sln", "./"]
COPY ["Shared/BuildingBlocks/CRM.BuildingBlocks/CRM.BuildingBlocks.csproj", "Shared/BuildingBlocks/CRM.BuildingBlocks/"]
COPY ["Services/Authentication/CRM.Authentication.Api/CRM.Authentication.Api.csproj", "Services/Authentication/CRM.Authentication.Api/"]
COPY ["Services/Authentication/CRM.Authentication.Application/CRM.Authentication.Application.csproj", "Services/Authentication/CRM.Authentication.Application/"]
COPY ["Services/Authentication/CRM.Authentication.Domain/CRM.Authentication.Domain.csproj", "Services/Authentication/CRM.Authentication.Domain/"]
COPY ["Services/Authentication/CRM.Authentication.Infrastructure/CRM.Authentication.Infrastructure.csproj", "Services/Authentication/CRM.Authentication.Infrastructure/"]

RUN dotnet restore "Services/Authentication/CRM.Authentication.Api/CRM.Authentication.Api.csproj"

# Sao chép toàn bộ source code
COPY . .

# Build dự án
WORKDIR "/src/Services/Authentication/CRM.Authentication.Api"
RUN dotnet build "CRM.Authentication.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CRM.Authentication.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CRM.Authentication.Api.dll"]
