# Perfect CV Admin Portal

Server-side rendered admin panel for Perfect CV using ASP.NET Core MVC with session-based authentication.

## Features

- 🔐 **Session-Based Authentication** - Secure cookie-based authentication using existing AuthService
- 👥 **Role-Based Access** - Only users with Admin role can access the portal
- 🎨 **Modern UI** - Clean, responsive design with Perfect CV branding
- 📊 **Dashboard** - Real-time statistics and metrics
- 🔄 **Integrated Services** - Uses existing Application layer services for business logic

## Tech Stack

- **Framework**: ASP.NET Core 8.0 MVC
- **Authentication**: Cookie-based sessions
- **Database**: PostgreSQL via Entity Framework Core
- **Caching**: Redis
- **UI**: Bootstrap 5 + Custom CSS

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- PostgreSQL database
- Redis server (optional, for refresh tokens)

### Configuration

Update `appsettings.json` with your settings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=service-perfect-cv;Username=postgres;Password=postgres123"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-here",
    "Issuer": "PerfectCVAdmin",
    "Audience": "PerfectCVAdmin"
  }
}
```

### Running the Application

```bash
cd ServicePerfectCV.WebAdmin
dotnet restore
dotnet run
```

Navigate to `https://localhost:5001` (or the port shown in terminal)

### Default Admin Login

To create an admin user, you need to:

1. Register a user through the API
2. Update the user's role to `Admin` in the database:

```sql
UPDATE "Users" SET "Role" = 1 WHERE "Email" = 'admin@perfectcv.com';
UPDATE "Users" SET "Status" = 1 WHERE "Email" = 'admin@perfectcv.com';
```

Then login with:
- Email: `admin@perfectcv.com`
- Password: (your registered password)

## Architecture

### Separation of Concerns

- **Controllers**: Handle HTTP requests and responses, manage session cookies
- **Views**: Razor pages for UI rendering
- **Models**: View models for data transfer
- **Business Logic**: Delegated to Application layer services (AuthService, UserService, etc.)

### Authentication Flow

1. User submits login form
2. `AuthController` validates input
3. Calls `AuthService.LoginAsync()` to authenticate
4. Verifies user has Admin role
5. Creates session cookie with claims
6. Redirects to dashboard

### Project Structure

```
ServicePerfectCV.WebAdmin/
├── Controllers/
│   ├── AuthController.cs      # Login/Logout
│   └── HomeController.cs      # Dashboard
├── Models/
│   ├── LoginViewModel.cs      # Login form model
│   └── ErrorViewModel.cs
├── Views/
│   ├── Auth/
│   │   └── Login.cshtml       # Login page
│   ├── Home/
│   │   └── Index.cshtml       # Dashboard
│   └── Shared/
│       ├── _AdminLayout.cshtml # Admin layout
│       └── _Layout.cshtml
├── Extensions/
│   └── ServiceExtensions.cs   # DI configuration
└── wwwroot/
    ├── css/
    │   ├── admin.css          # Admin panel styles
    │   └── admin-login.css    # Login page styles
    └── js/
        └── admin.js           # Admin panel JavaScript
```

## Security Features

- ✅ HTTPS enforced
- ✅ HttpOnly cookies
- ✅ SameSite cookie policy
- ✅ CSRF protection with anti-forgery tokens
- ✅ Role-based authorization
- ✅ Secure password hashing (handled by AuthService)
- ✅ Session timeout (8 hours default)

## Future Enhancements

- [ ] User management interface
- [ ] CV management and moderation
- [ ] Job listing management
- [ ] Analytics and reporting
- [ ] System settings configuration
- [ ] Audit logs
- [ ] Two-factor authentication

## License

© 2024 Perfect CV. All rights reserved.

