FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["CRM.sln", "./"]
COPY ["Shared/BuildingBlocks/CRM.BuildingBlocks/CRM.BuildingBlocks.csproj", "Shared/BuildingBlocks/CRM.BuildingBlocks/"]
COPY ["Services/HR/CRM.HR.Api/CRM.HR.Api.csproj", "Services/HR/CRM.HR.Api/"]
COPY ["Services/HR/CRM.HR.Application/CRM.HR.Application.csproj", "Services/HR/CRM.HR.Application/"]
COPY ["Services/HR/CRM.HR.Domain/CRM.HR.Domain.csproj", "Services/HR/CRM.HR.Domain/"]
COPY ["Services/HR/CRM.HR.Infrastructure/CRM.HR.Infrastructure.csproj", "Services/HR/CRM.HR.Infrastructure/"]

RUN dotnet restore "Services/HR/CRM.HR.Api/CRM.HR.Api.csproj"
COPY . .
WORKDIR "/src/Services/HR/CRM.HR.Api"
RUN dotnet build "CRM.HR.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CRM.HR.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CRM.HR.Api.dll"]
