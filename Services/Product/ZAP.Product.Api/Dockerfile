FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["ZAP.sln", "./"]
COPY ["Shared/BuildingBlocks/ZAP.BuildingBlocks/ZAP.BuildingBlocks.csproj", "Shared/BuildingBlocks/ZAP.BuildingBlocks/"]
COPY ["Services/Product/ZAP.Product.Api/ZAP.Product.Api.csproj", "Services/Product/ZAP.Product.Api/"]
COPY ["Services/Product/ZAP.Product.Application/ZAP.Product.Application.csproj", "Services/Product/ZAP.Product.Application/"]
COPY ["Services/Product/ZAP.Product.Domain/ZAP.Product.Domain.csproj", "Services/Product/ZAP.Product.Domain/"]
COPY ["Services/Product/ZAP.Product.Infrastructure/ZAP.Product.Infrastructure.csproj", "Services/Product/ZAP.Product.Infrastructure/"]

RUN dotnet restore "Services/Product/ZAP.Product.Api/ZAP.Product.Api.csproj"

COPY . .

WORKDIR "/src/Services/Product/ZAP.Product.Api"
RUN dotnet build "ZAP.Product.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ZAP.Product.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZAP.Product.Api.dll"]
