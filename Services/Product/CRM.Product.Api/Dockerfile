FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Sao chép các tệp project và restore
COPY ["CRM.sln", "./"]
COPY ["Shared/BuildingBlocks/CRM.BuildingBlocks/CRM.BuildingBlocks.csproj", "Shared/BuildingBlocks/CRM.BuildingBlocks/"]
COPY ["Services/Product/CRM.Product.Api/CRM.Product.Api.csproj", "Services/Product/CRM.Product.Api/"]
COPY ["Services/Product/CRM.Product.Application/CRM.Product.Application.csproj", "Services/Product/CRM.Product.Application/"]
COPY ["Services/Product/CRM.Product.Domain/CRM.Product.Domain.csproj", "Services/Product/CRM.Product.Domain/"]
COPY ["Services/Product/CRM.Product.Infrastructure/CRM.Product.Infrastructure.csproj", "Services/Product/CRM.Product.Infrastructure/"]

RUN dotnet restore "Services/Product/CRM.Product.Api/CRM.Product.Api.csproj"

# Sao chép toàn bộ source code
COPY . .

# Build dự án
WORKDIR "/src/Services/Product/CRM.Product.Api"
RUN dotnet build "CRM.Product.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CRM.Product.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CRM.Product.Api.dll"]
