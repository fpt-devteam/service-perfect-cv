FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ServicePerfectCV.Domain/ServicePerfectCV.Domain.csproj ServicePerfectCV.Domain/
COPY ServicePerfectCV.Application/ServicePerfectCV.Application.csproj ServicePerfectCV.Application/
COPY ServicePerfectCV.Infrastructure/ServicePerfectCV.Infrastructure.csproj ServicePerfectCV.Infrastructure/
COPY ServicePerfectCV.WebApi/ServicePerfectCV.WebApi.csproj ServicePerfectCV.WebApi/

RUN dotnet restore ServicePerfectCV.WebApi/ServicePerfectCV.WebApi.csproj

COPY . ./

RUN dotnet publish ServicePerfectCV.WebApi/ServicePerfectCV.WebApi.csproj \
    -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080}
EXPOSE 8080

ENTRYPOINT ["dotnet", "ServicePerfectCV.WebApi.dll"]
