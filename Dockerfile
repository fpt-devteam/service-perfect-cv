FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files first to optimize Docker caching
COPY ["src/ServicePerfectCV.WebApi/ServicePerfectCV.WebApi.csproj", "src/ServicePerfectCV.WebApi/"]
COPY ["src/ServicePerfectCV.Application/ServicePerfectCV.Application.csproj", "src/ServicePerfectCV.Application/"]
COPY ["src/ServicePerfectCV.Domain/ServicePerfectCV.Domain.csproj", "src/ServicePerfectCV.Domain/"]
COPY ["src/ServicePerfectCV.Infrastructure/ServicePerfectCV.Infrastructure.csproj", "src/ServicePerfectCV.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/ServicePerfectCV.WebApi/ServicePerfectCV.WebApi.csproj"

# Copy the rest of the code
COPY . .

# Build the project
WORKDIR "/src/src/ServicePerfectCV.WebApi"
RUN dotnet build "ServicePerfectCV.WebApi.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "ServicePerfectCV.WebApi.csproj" -c Release -o /app/publish

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
EXPOSE 443

# Create entrypoint script to handle environment variables
RUN echo '#!/bin/bash\n\
    echo "$ENV_VARS" > .env\n\
    dotnet ServicePerfectCV.WebApi.dll' > /app/entrypoint.sh

RUN chmod +x /app/entrypoint.sh
ENTRYPOINT ["/app/entrypoint.sh"]