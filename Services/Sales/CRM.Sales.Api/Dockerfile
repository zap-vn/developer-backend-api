FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["CRM.sln", "./"]
COPY ["Shared/BuildingBlocks/CRM.BuildingBlocks/CRM.BuildingBlocks.csproj", "Shared/BuildingBlocks/CRM.BuildingBlocks/"]
COPY ["Services/Sales/CRM.Sales.Api/CRM.Sales.Api.csproj", "Services/Sales/CRM.Sales.Api/"]
COPY ["Services/Sales/CRM.Sales.Application/CRM.Sales.Application.csproj", "Services/Sales/CRM.Sales.Application/"]
COPY ["Services/Sales/CRM.Sales.Domain/CRM.Sales.Domain.csproj", "Services/Sales/CRM.Sales.Domain/"]
COPY ["Services/Sales/CRM.Sales.Infrastructure/CRM.Sales.Infrastructure.csproj", "Services/Sales/CRM.Sales.Infrastructure/"]

RUN dotnet restore "Services/Sales/CRM.Sales.Api/CRM.Sales.Api.csproj"
COPY . .
WORKDIR "/src/Services/Sales/CRM.Sales.Api"
RUN dotnet build "CRM.Sales.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CRM.Sales.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CRM.Sales.Api.dll"]
