# Stage 1: Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Stage 2: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files (assuming Railway root is service-cv-management)
COPY ["./ServicePerfectCV.WebApi/ServicePerfectCV.WebApi.csproj", "./ServicePerfectCV.WebApi/"]
COPY ["./ServicePerfectCV.Application/ServicePerfectCV.Application.csproj", "./ServicePerfectCV.Application/"]
COPY ["./ServicePerfectCV.Domain/ServicePerfectCV.Domain.csproj", "./ServicePerfectCV.Domain/"]
COPY ["./ServicePerfectCV.Infrastructure/ServicePerfectCV.Infrastructure.csproj", "./ServicePerfectCV.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "./ServicePerfectCV.WebApi/ServicePerfectCV.WebApi.csproj"

# Copy all source files
COPY . .

# Build the project
WORKDIR "/src/ServicePerfectCV.WebApi"
RUN dotnet build "ServicePerfectCV.WebApi.csproj" -c Release -o /app/build

# Stage 3: Publish
FROM build AS publish
RUN dotnet publish "ServicePerfectCV.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 4: Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ServicePerfectCV.WebApi.dll"]
