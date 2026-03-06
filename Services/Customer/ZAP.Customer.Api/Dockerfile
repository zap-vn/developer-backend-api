FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Sao chép các tệp project và restore
COPY ["ZAP.sln", "./"]
COPY ["Shared/BuildingBlocks/ZAP.BuildingBlocks/ZAP.BuildingBlocks.csproj", "Shared/BuildingBlocks/ZAP.BuildingBlocks/"]
COPY ["Services/Customer/ZAP.Customer.Api/ZAP.Customer.Api.csproj", "Services/Customer/ZAP.Customer.Api/"]
COPY ["Services/Customer/ZAP.Customer.Application/ZAP.Customer.Application.csproj", "Services/Customer/ZAP.Customer.Application/"]
COPY ["Services/Customer/ZAP.Customer.Domain/ZAP.Customer.Domain.csproj", "Services/Customer/ZAP.Customer.Domain/"]
COPY ["Services/Customer/ZAP.Customer.Infrastructure/ZAP.Customer.Infrastructure.csproj", "Services/Customer/ZAP.Customer.Infrastructure/"]

RUN dotnet restore "Services/Customer/ZAP.Customer.Api/ZAP.Customer.Api.csproj"

# Sao chép toàn bộ source code
COPY . .

# Build dự án
WORKDIR "/src/Services/Customer/ZAP.Customer.Api"
RUN dotnet build "ZAP.Customer.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ZAP.Customer.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZAP.Customer.Api.dll"]
