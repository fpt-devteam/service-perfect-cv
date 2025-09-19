# service-template

Backend API service for web and mobile applications.

## Table of Contents

- [Run Database with Docker Compose](#run-database-with-docker-compose)
- [Database Migration](#database-migration)
- [Seed Data](#seed-data)
- [Documentation](#documentation)
- [Code Formatting](#code-formatting)

---

## Run Database with Docker Compose

The project database is pre-configured using Docker Compose for easy startup and management.

### Start Database

```bash
docker-compose up -d
```

This command will create and run the database container in the background.

### Check Database Status

```bash
docker-compose logs db
```

This command shows the logs of the database service to verify if it started successfully.

### Stop Database and Remove Data Volume (if needed)

```bash
docker-compose down -v
```

This command stops all containers and deletes the data volume (note: database data will be lost).

## Database Migration

Migration helps update the database schema (tables, columns, indexes, etc.) based on the data models in your code.

### Important: PostgreSQL Migration

The project has been migrated from SQL Server to PostgreSQL. If you have existing migrations from SQL Server, follow these steps to clean up:

#### Clean Up Old Migrations (One-time setup)

1. **Remove existing migrations folder:**
   ```bash
   del /q src/service-cv-management/ServicePerfectCV.Infrastructure/Migrations
   ```

2. **Create initial migration for PostgreSQL:**
   ```bash
   dotnet ef migrations add InitialPostgreSQL --project ./src/service-cv-management/ServicePerfectCV.Infrastructure --startup-project ./src/service-cv-management/ServicePerfectCV.WebApi
   ```

3. **Update database to apply new migration:**
   ```bash
   dotnet ef database update --project ./src/service-cv-management/ServicePerfectCV.Infrastructure --startup-project ./src/service-cv-management/ServicePerfectCV.WebApi
   ```

### Regular Migration Commands

#### Add a New Migration

```bash
dotnet ef migrations add <MigrationName> --project ./src/service-cv-management/ServicePerfectCV.Infrastructure --startup-project ./src/service-cv-management/ServicePerfectCV.WebApi
```

#### Apply Migration to Database

```bash
dotnet ef database update --project ./src/service-cv-management/ServicePerfectCV.Infrastructure --startup-project ./src/service-cv-management/ServicePerfectCV.WebApi
```

This command creates or updates the database schema to the latest migration.

#### Remove Last Migration (if you need to fix it)

```bash
dotnet ef migrations remove --project ./src/service-cv-management/ServicePerfectCV.Infrastructure --startup-project ./src/service-cv-management/ServicePerfectCV.WebApi
```

## Seed Data

Seed data inserts sample or initial data into the database for testing or bootstrapping.

### Configure Seed Data

Open appsettings.json

Find the Seed section and set Enabled to true to enable seeding on project start.

Example:

```bash
"Seed": {
  "Enabled": true
}

```

### Run Project to Seed Data

When running the project with Seed.Enabled = true, sample data will be automatically inserted into the database.

### Disable Seed Data

To disable seeding, set Seed.Enabled to false and restart the project.

## Documentation

Build and run the `api` project should also generate `swagger` spec from Micronaut annotations at:

- Swagger local http://localhost:8080/swagger/index.html
- Swagger dev https://perfect-cv-ghd4h9b6f5d3atcq.southeastasia-01.azurewebsites.net/swagger/index.html

## Code Formatting

Before committing and pushing your code to Git, please ensure your code is properly formatted.

To format the code, run the following command in your terminal **at the root folder where the solution (.sln) file is located**:

```bash
dotnet format
```
