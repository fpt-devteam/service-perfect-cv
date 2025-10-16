# ===================== Build stage =====================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files (relative to this directory)
COPY ServicePerfectCV.Domain/ServicePerfectCV.Domain.csproj ServicePerfectCV.Domain/
COPY ServicePerfectCV.Application/ServicePerfectCV.Application.csproj ServicePerfectCV.Application/
COPY ServicePerfectCV.Infrastructure/ServicePerfectCV.Infrastructure.csproj ServicePerfectCV.Infrastructure/
COPY ServicePerfectCV.WebApi/ServicePerfectCV.WebApi.csproj ServicePerfectCV.WebApi/

# Restore dependencies
RUN dotnet restore ServicePerfectCV.WebApi/ServicePerfectCV.WebApi.csproj

# Copy all the source code
COPY . ./

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
