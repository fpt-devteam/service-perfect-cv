# ===================== Build stage =====================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files (from project root via railway.toml)
COPY ./src/service-cv-management/ServicePerfectCV.Domain/ServicePerfectCV.Domain.csproj ServicePerfectCV.Domain/
COPY ./src/service-cv-management/ServicePerfectCV.Application/ServicePerfectCV.Application.csproj ServicePerfectCV.Application/
COPY ./src/service-cv-management/ServicePerfectCV.Infrastructure/ServicePerfectCV.Infrastructure.csproj ServicePerfectCV.Infrastructure/
COPY ./src/service-cv-management/ServicePerfectCV.WebApi/ServicePerfectCV.WebApi.csproj ServicePerfectCV.WebApi/

# Restore dependencies
RUN dotnet restore ServicePerfectCV.WebApi/ServicePerfectCV.WebApi.csproj

# Copy all the source code from service-cv-management
COPY ./src/service-cv-management/ ./

# Build and publish
RUN dotnet publish ServicePerfectCV.WebApi/ServicePerfectCV.WebApi.csproj \
  -c Release -o /app/publish /p:UseAppHost=false

# ===================== Runtime stage =====================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

# Listen on port 8080 (Railway automatically maps this)
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ServicePerfectCV.WebApi.dll"]
