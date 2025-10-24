# syntax=docker/dockerfile:1.6

# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# NOTE:
# This Dockerfile assumes the Docker build CONTEXT contains the sibling projects
# (ServicePerfectCV.Application, ServicePerfectCV.Infrastructure, ServicePerfectCV.Domain)
# in the parent directory. If your platform limits the context to only this folder,
# set your Railway service Root Directory to "src/service-cv-management" instead,
# or use the monorepo Dockerfile at src/service-cv-management/Dockerfile.

# 1) Copy project files for layer-cached restore
COPY ServicePerfectCV.WebAdmin.csproj ServicePerfectCV.WebAdmin/
COPY ../ServicePerfectCV.Application/ServicePerfectCV.Application.csproj ServicePerfectCV.Application/
COPY ../ServicePerfectCV.Infrastructure/ServicePerfectCV.Infrastructure.csproj ServicePerfectCV.Infrastructure/
COPY ../ServicePerfectCV.Domain/ServicePerfectCV.Domain.csproj ServicePerfectCV.Domain/

# 2) Restore dependencies (based on project references)
RUN dotnet restore ServicePerfectCV.WebAdmin/ServicePerfectCV.WebAdmin.csproj

# 3) Copy source code
COPY . ServicePerfectCV.WebAdmin/
COPY ../ServicePerfectCV.Application/ ServicePerfectCV.Application/
COPY ../ServicePerfectCV.Infrastructure/ ServicePerfectCV.Infrastructure/
COPY ../ServicePerfectCV.Domain/ ServicePerfectCV.Domain/

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
