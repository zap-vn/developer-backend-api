FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["CRM.sln", "./"]
COPY ["Shared/BuildingBlocks/CRM.BuildingBlocks/CRM.BuildingBlocks.csproj", "Shared/BuildingBlocks/CRM.BuildingBlocks/"]
COPY ["Services/Order/CRM.Order.Api/CRM.Order.Api.csproj", "Services/Order/CRM.Order.Api/"]
COPY ["Services/Order/CRM.Order.Application/CRM.Order.Application.csproj", "Services/Order/CRM.Order.Application/"]
COPY ["Services/Order/CRM.Order.Domain/CRM.Order.Domain.csproj", "Services/Order/CRM.Order.Domain/"]
COPY ["Services/Order/CRM.Order.Infrastructure/CRM.Order.Infrastructure.csproj", "Services/Order/CRM.Order.Infrastructure/"]

RUN dotnet restore "Services/Order/CRM.Order.Api/CRM.Order.Api.csproj"
COPY . .
WORKDIR "/src/Services/Order/CRM.Order.Api"
RUN dotnet build "CRM.Order.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CRM.Order.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CRM.Order.Api.dll"]
