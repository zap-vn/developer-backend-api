FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Sao chép các tệp project và restore
COPY ["ZAP.sln", "./"]
COPY ["Shared/BuildingBlocks/ZAP.BuildingBlocks/ZAP.BuildingBlocks.csproj", "Shared/BuildingBlocks/ZAP.BuildingBlocks/"]
COPY ["Services/Authentication/ZAP.Authentication.Api/ZAP.Authentication.Api.csproj", "Services/Authentication/ZAP.Authentication.Api/"]
COPY ["Services/Authentication/ZAP.Authentication.Application/ZAP.Authentication.Application.csproj", "Services/Authentication/ZAP.Authentication.Application/"]
COPY ["Services/Authentication/ZAP.Authentication.Domain/ZAP.Authentication.Domain.csproj", "Services/Authentication/ZAP.Authentication.Domain/"]
COPY ["Services/Authentication/ZAP.Authentication.Infrastructure/ZAP.Authentication.Infrastructure.csproj", "Services/Authentication/ZAP.Authentication.Infrastructure/"]

RUN dotnet restore "Services/Authentication/ZAP.Authentication.Api/ZAP.Authentication.Api.csproj"

# Sao chép toàn bộ source code
COPY . .

# Build dự án
WORKDIR "/src/Services/Authentication/ZAP.Authentication.Api"
RUN dotnet build "ZAP.Authentication.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ZAP.Authentication.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZAP.Authentication.Api.dll"]
