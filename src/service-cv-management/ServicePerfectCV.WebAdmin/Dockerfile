# syntax=docker/dockerfile:1.6

# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ServicePerfectCV.WebAdmin.csproj ./
RUN dotnet restore "ServicePerfectCV.WebAdmin.csproj"

# Copy everything and publish
COPY . .
RUN dotnet publish "ServicePerfectCV.WebAdmin.csproj" -c Release -o /app/publish /p:UseAppHost=false

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
