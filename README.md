# TalentoPlus - Employee Management System

A comprehensive employee management system built with ASP.NET Core, featuring both a Web MVC application and a REST API with JWT authentication.

## 🏗️ Architecture

The project follows Clean Architecture principles with the following structure:

- **TalentoPlus.Core** - Domain entities and DTOs
- **TalentoPlus.Infrastructure** - Data access, repositories, and services
- **TalentoPlus.Web** - ASP.NET Core MVC web application
- **TalentoPlus.API** - RESTful API with Swagger documentation

## 🚀 Features

### Web Application
- Employee CRUD operations
- Department management
- Dashboard with analytics
- Excel export functionality
- PDF CV generation
- User authentication with ASP.NET Identity

### REST API
- JWT-based authentication
- Role-based authorization (Admin/Employee)
- Swagger/OpenAPI documentation
- Employee self-service endpoints
- Admin management endpoints

## 📋 Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) and Docker Compose
- PostgreSQL (Supabase is used in this project)

## 🔧 Configuration

### Database Connection

The project uses Supabase (PostgreSQL) as the database. Connection strings are configured in:

- `TalentoPlus.Web/appsettings.json`
- `TalentoPlus.API/appsettings.json`
- `docker-compose.yml`

Current configuration:
```
Host: aws-0-us-west-2.pooler.supabase.com
Port: 6543
Database: postgres
```

### Environment Variables

The `.env` file contains sensitive configuration (already configured):
- Database credentials
- JWT secret key
- SMTP settings
- Gemini API key

> **Note**: The `.env` file is ignored by git for security.

## 🐳 Running with Docker (Recommended)

### 1. Build and Start Containers

```bash
docker-compose up --build
```

This will start:
- **Web Application** on `http://localhost:1924`
- **API** on `http://localhost:1925`
- **Swagger UI** on `http://localhost:1925/swagger`

### 2. Stop Containers

```bash
docker-compose down
```

### 3. View Logs

```bash
docker-compose logs -f
```

## 💻 Running Locally (Development)

### 1. Restore Dependencies

```bash
dotnet restore
```

### 2. Run Database Migrations

```bash
cd TalentoPlus.Web
dotnet ef database update
```

### 3. Run the Web Application

```bash
cd TalentoPlus.Web
dotnet run
```

Access at: `http://localhost:5173`

### 4. Run the API

```bash
cd TalentoPlus.API
dotnet run
```

Access at: `http://localhost:5255`
Swagger UI: `http://localhost:5255/swagger`

## 🔐 Default Credentials

After the first run, the database is seeded with:

**Admin User:**
- Email: `admin@talentoplus.com`
- Password: `Admin123!`
- Role: Admin

**Sample Employees:**
- Alice Smith (Senior Developer)
- Bob Jones (HR Manager)
- Charlie Brown (Junior Developer)

## 📚 API Documentation

### Authentication

#### Login
```bash
POST /api/Auth/login
Content-Type: application/json

{
  "email": "admin@talentoplus.com",
  "password": "Admin123!"
}
```

Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

#### Register Employee
```bash
POST /api/Auth/register
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@talentoplus.com",
  "documentNumber": "1004",
  "departmentId": 1
}
```

### Using the API with Authentication

1. **Get a token** by logging in via `/api/Auth/login`
2. **Authorize in Swagger**:
   - Click the **Authorize** button (🔒)
   - Enter: `Bearer YOUR_TOKEN_HERE`
   - Click **Authorize** and **Close**
3. **Make authenticated requests**

### Employee Endpoints

#### Get My Profile (Authenticated User)
```bash
GET /api/Employees/me
Authorization: Bearer YOUR_TOKEN
```

#### Download My CV (Authenticated User)
```bash
GET /api/Employees/me/cv
Authorization: Bearer YOUR_TOKEN
```

### Admin Endpoints (Require Admin Role)

#### Get All Employees
```bash
GET /api/Employees
Authorization: Bearer YOUR_TOKEN
```

#### Get Employee by ID
```bash
GET /api/Employees/{id}
Authorization: Bearer YOUR_TOKEN
```

#### Create Employee
```bash
POST /api/Employees
Authorization: Bearer YOUR_TOKEN
Content-Type: application/json

{
  "firstName": "Jane",
  "lastName": "Smith",
  "email": "jane.smith@talentoplus.com",
  "documentNumber": "1005",
  "position": "Software Engineer",
  "salary": 5000,
  "joinDate": "2024-01-15",
  "status": "Active",
  "educationLevel": "Bachelor",
  "professionalProfile": "Full Stack Developer",
  "contactPhone": "555-0104",
  "departmentId": 1
}
```

#### Update Employee
```bash
PUT /api/Employees/{id}
Authorization: Bearer YOUR_TOKEN
Content-Type: application/json

{
  "firstName": "Jane",
  "lastName": "Smith",
  ...
}
```

#### Delete Employee
```bash
DELETE /api/Employees/{id}
Authorization: Bearer YOUR_TOKEN
```

## 🛠️ Technologies Used

### Backend
- ASP.NET Core 8.0
- Entity Framework Core
- PostgreSQL (via Supabase)
- ASP.NET Identity
- JWT Authentication

### Frontend (Web)
- ASP.NET Core MVC
- Razor Views
- Bootstrap

### Libraries & Tools
- Swashbuckle (Swagger/OpenAPI)
- EPPlus (Excel generation)
- QuestPDF (PDF generation)
- Docker & Docker Compose

## 📁 Project Structure

```
TalentoPlus/
├── TalentoPlus.Core/
│   ├── Entities/          # Domain models
│   └── DTOs/              # Data transfer objects
├── TalentoPlus.Infrastructure/
│   ├── Data/              # DbContext and migrations
│   ├── Repositories/      # Data access layer
│   └── Services/          # Business logic services
├── TalentoPlus.Web/
│   ├── Controllers/       # MVC controllers
│   ├── Views/             # Razor views
│   └── Dockerfile
├── TalentoPlus.API/
│   ├── Controllers/       # API controllers
│   └── Dockerfile
├── docker-compose.yml
├── .env                   # Environment variables (gitignored)
└── README.md
```

## 🔒 Security Features

- **JWT Authentication** - Secure token-based authentication
- **Role-Based Authorization** - Admin and Employee roles
- **Password Hashing** - ASP.NET Identity password security
- **CORS Enabled** - Configured for API access
- **Environment Variables** - Sensitive data in `.env` (gitignored)

## 🐛 Troubleshooting

### Port Already in Use

If you get "address already in use" errors:

```bash
# Find and kill process using the port
lsof -i :5255
kill -9 <PID>
```

### Docker Issues

```bash
# Clean up Docker resources
docker-compose down
docker system prune -a

# Rebuild from scratch
docker-compose up --build
```

### Database Connection Issues

1. Verify Supabase credentials in `appsettings.json`
2. Check network connectivity
3. Ensure SSL mode is set to `Require`

### Swagger Not Loading

- Swagger is now enabled in **all environments** (including Production)
- Access at: `http://localhost:1925/swagger` (Docker) or `http://localhost:5255/swagger` (local)

## 📝 Development Notes

### Docker Configuration

- **Web App**: Runs on port `1924` (mapped from internal `8080`)
- **API**: Runs on port `1925` (mapped from internal `8080`)
- **Restart Policy**: `unless-stopped` for both services
- **No local PostgreSQL**: Uses remote Supabase database

### Production Considerations

- HTTPS redirection is disabled in Docker (uses HTTP on port 8080)
- Swagger is enabled in production for API documentation
- CORS is configured to allow any origin (adjust for production)
- Database migrations run automatically on startup

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## 📄 License

This project is licensed under the MIT License.

## 👥 Authors

- Johandry Julio

## 📞 Support

For issues and questions, please open an issue in the repository.
