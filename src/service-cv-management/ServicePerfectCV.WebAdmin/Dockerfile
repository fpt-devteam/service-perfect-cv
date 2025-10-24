# syntax=docker/dockerfile:1.6

# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/service-cv-management/ServicePerfectCV.Domain/ServicePerfectCV.Domain.csproj ServicePerfectCV.Domain/
COPY src/service-cv-management/ServicePerfectCV.Application/ServicePerfectCV.Application.csproj ServicePerfectCV.Application/
COPY src/service-cv-management/ServicePerfectCV.Infrastructure/ServicePerfectCV.Infrastructure.csproj ServicePerfectCV.Infrastructure/
COPY src/service-cv-management/ServicePerfectCV.WebAdmin/ServicePerfectCV.WebAdmin.csproj ServicePerfectCV.WebAdmin/


# 2) Restore dependencies (based on project references)
RUN dotnet restore ServicePerfectCV.WebAdmin/ServicePerfectCV.WebAdmin.csproj

# 3) Copy source code
COPY ./src/service-cv-management/ ./

# 4) Publish
RUN dotnet publish ServicePerfectCV.WebAdmin/ServicePerfectCV.WebAdmin.csproj -c Release -o /app/publish /p:UseAppHost=false

# ---- Run stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Railway provides PORT; bind Kestrel to it (fallback 8080 for local)
ENV PORT=8080
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
ENV ASPNETCORE_ENVIRONMENT=Production

# Copy published app
COPY --from=build /app/publish .

# Expose the port for local runs (Railway injects PORT automatically)
EXPOSE 8080

# Start
ENTRYPOINT ["dotnet", "ServicePerfectCV.WebAdmin.dll"]
