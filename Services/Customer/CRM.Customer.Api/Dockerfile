FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Sao chép các tệp project và restore
COPY ["CRM.sln", "./"]
COPY ["Shared/BuildingBlocks/CRM.BuildingBlocks/CRM.BuildingBlocks.csproj", "Shared/BuildingBlocks/CRM.BuildingBlocks/"]
COPY ["Services/Customer/CRM.Customer.Api/CRM.Customer.Api.csproj", "Services/Customer/CRM.Customer.Api/"]
COPY ["Services/Customer/CRM.Customer.Application/CRM.Customer.Application.csproj", "Services/Customer/CRM.Customer.Application/"]
COPY ["Services/Customer/CRM.Customer.Domain/CRM.Customer.Domain.csproj", "Services/Customer/CRM.Customer.Domain/"]
COPY ["Services/Customer/CRM.Customer.Infrastructure/CRM.Customer.Infrastructure.csproj", "Services/Customer/CRM.Customer.Infrastructure/"]

RUN dotnet restore "Services/Customer/CRM.Customer.Api/CRM.Customer.Api.csproj"

# Sao chép toàn bộ source code
COPY . .

# Build dự án
WORKDIR "/src/Services/Customer/CRM.Customer.Api"
RUN dotnet build "CRM.Customer.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CRM.Customer.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CRM.Customer.Api.dll"]
