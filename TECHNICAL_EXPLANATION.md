# TalentoPlus - Explicación Profunda del Proyecto

## 1. VISIÓN GENERAL DEL SISTEMA

TalentoPlus es un sistema integral de gestión de recursos humanos diseñado con una arquitectura moderna y escalable. El proyecto implementa dos interfaces principales: una aplicación web MVC para usuarios internos de la empresa y una API RESTful para integración con otros sistemas o aplicaciones móviles.

### Propósito del Sistema
El sistema permite a las organizaciones:
- Gestionar información completa de empleados (datos personales, profesionales, departamentales)
- Controlar accesos mediante autenticación y autorización basada en roles
- Generar reportes y documentos (CVs en PDF, exportaciones a Excel)
- Proporcionar autoservicio a empleados para consultar su información
- Administrar departamentos y estructuras organizacionales

## 2. ARQUITECTURA DEL PROYECTO

### 2.1 Clean Architecture (Arquitectura Limpia)

El proyecto sigue los principios de Clean Architecture, separando las responsabilidades en capas bien definidas:

**TalentoPlus.Core (Capa de Dominio)**
- Contiene las entidades del negocio (Employee, Department, User)
- Define los DTOs (Data Transfer Objects) para transferencia de datos
- No tiene dependencias externas, es el núcleo puro del negocio
- Representa el "qué" del sistema (qué es un empleado, qué es un departamento)

**TalentoPlus.Infrastructure (Capa de Infraestructura)**
- Implementa el acceso a datos mediante Entity Framework Core
- Contiene el DbContext (ApplicationDbContext) que mapea las entidades a la base de datos
- Implementa los repositorios (EmployeeRepository) siguiendo el patrón Repository
- Provee servicios de infraestructura (EmailService, PdfService, ExcelService)
- Gestiona las migraciones de base de datos
- Representa el "cómo" del sistema (cómo se guardan los datos, cómo se envían emails)

**TalentoPlus.Web (Capa de Presentación - MVC)**
- Aplicación web tradicional con patrón MVC (Model-View-Controller)
- Controllers manejan las peticiones HTTP y coordinan la lógica
- Views (Razor) renderizan HTML dinámico en el servidor
- Autenticación mediante ASP.NET Identity con cookies
- Interfaz visual para operaciones CRUD de empleados y departamentos

**TalentoPlus.API (Capa de Presentación - REST API)**
- API RESTful que expone endpoints HTTP
- Autenticación mediante JWT (JSON Web Tokens)
- Documentación automática con Swagger/OpenAPI
- Diseñada para consumo por aplicaciones externas o frontends SPA

### 2.2 Flujo de Datos

```
Usuario → Controller → Repository → DbContext → Base de Datos
                ↓
            Service (Email, PDF, Excel)
```

1. El usuario hace una petición (web o API)
2. El Controller recibe la petición y valida los datos
3. El Controller llama al Repository para operaciones de datos
4. El Repository usa el DbContext para ejecutar queries en la BD
5. Los Services se invocan para operaciones adicionales (generar PDF, enviar email)
6. La respuesta se devuelve al usuario

## 3. COMPONENTES TÉCNICOS DETALLADOS

### 3.1 Sistema de Autenticación y Autorización

**ASP.NET Identity**
- Framework de Microsoft para gestión de usuarios
- Maneja el hash seguro de contraseñas (PBKDF2)
- Gestiona roles (Admin, Employee)
- Almacena usuarios en tablas específicas (AspNetUsers, AspNetRoles, AspNetUserRoles)

**JWT (JSON Web Tokens) para la API**
- Token firmado digitalmente que contiene claims (información del usuario)
- Estructura: Header.Payload.Signature
- El Payload contiene: ID de usuario, email, roles
- Firmado con una clave secreta (JwtSettings:Secret)
- Válido por 7 días
- Stateless: no requiere almacenamiento en servidor

**Flujo de Autenticación API:**
1. Usuario envía credenciales a POST /api/Auth/login
2. Sistema valida usuario y contraseña con Identity
3. Si es válido, genera un JWT con los claims del usuario
4. Cliente almacena el token (localStorage, memoria)
5. Cliente envía el token en header: "Authorization: Bearer {token}"
6. Middleware de JWT valida el token en cada petición
7. Si es válido, establece el contexto de usuario (User.Claims)

**Autorización Basada en Roles:**
```csharp
[Authorize(Roles = "Admin")]  // Solo administradores
public async Task<IActionResult> Delete(int id)
```

### 3.2 Base de Datos (PostgreSQL via Supabase)

**Entity Framework Core**
- ORM (Object-Relational Mapper) que mapea objetos C# a tablas SQL
- Code-First approach: las entidades definen el esquema
- Migraciones automáticas para evolucionar el esquema

**Modelo de Datos Principal:**

```
User (ASP.NET Identity)
├── Id (string)
├── Email
├── PasswordHash
└── Roles (many-to-many)

Employee
├── Id (int, PK)
├── FirstName
├── LastName
├── Email
├── DocumentNumber
├── Position
├── Salary
├── JoinDate
├── Status (Active, Vacation, Inactive)
├── EducationLevel
├── ProfessionalProfile
├── ContactPhone
├── DepartmentId (FK)
├── UserId (FK, nullable)
└── Department (navigation property)

Department
├── Id (int, PK)
├── Name
└── Employees (collection)
```

**Relaciones:**
- Department → Employees (1:N)
- User → Employee (1:1, opcional)

**Supabase:**
- Servicio de PostgreSQL en la nube
- Conexión segura con SSL
- Pooling de conexiones para mejor rendimiento
- Backup automático

### 3.3 Servicios de Infraestructura

**EmailService**
- Usa SMTP (Simple Mail Transfer Protocol)
- Configurado para Gmail (smtp.gmail.com:587)
- Envía emails de bienvenida al registrar empleados
- Usa credenciales de aplicación (no contraseña normal)

**PdfService (QuestPDF)**
- Genera CVs en formato PDF
- Layout programático (no plantillas HTML)
- Incluye: foto, datos personales, experiencia, educación
- Descarga directa desde el navegador

**ExcelService (EPPlus)**
- Exporta listas de empleados a Excel (.xlsx)
- Formato con headers, estilos, filtros automáticos
- Útil para análisis de datos o reportes

### 3.4 Patrón Repository

**Propósito:**
- Abstrae el acceso a datos
- Facilita testing (se puede mockear)
- Centraliza queries complejas

**Ejemplo:**
```csharp
public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAllAsync();
    Task<Employee?> GetByIdAsync(int id);
    Task<Employee?> GetByEmailAsync(string email);
    Task AddAsync(Employee employee);
    Task UpdateAsync(Employee employee);
    Task DeleteAsync(int id);
}
```

**Implementación:**
```csharp
public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _context;
    
    public async Task<Employee?> GetByIdAsync(int id)
    {
        return await _context.Employees
            .Include(e => e.Department)  // Eager loading
            .FirstOrDefaultAsync(e => e.Id == id);
    }
}
```

## 4. DOCKER Y CONTAINERIZACIÓN

### 4.1 ¿Por qué Docker?

Docker permite empaquetar la aplicación con todas sus dependencias en contenedores aislados:
- **Consistencia:** Funciona igual en desarrollo, staging y producción
- **Portabilidad:** Se ejecuta en cualquier máquina con Docker
- **Aislamiento:** No interfiere con otras aplicaciones
- **Escalabilidad:** Fácil de replicar y escalar

### 4.2 Arquitectura Docker

**docker-compose.yml**
Define dos servicios:

1. **talentoplus_web** (Puerto 1924)
   - Construye desde TalentoPlus.Web/Dockerfile
   - Expone puerto 8080 internamente, mapeado a 1924 externamente
   - Variables de entorno inyectadas
   - Red compartida: talentoplus_network

2. **talentoplus_api** (Puerto 1925)
   - Construye desde TalentoPlus.API/Dockerfile
   - Expone puerto 8080 internamente, mapeado a 1925 externamente
   - Incluye configuración JWT
   - Red compartida: talentoplus_network

**Dockerfile (multi-stage build):**
```dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TalentoPlus.API.dll"]
```

**Ventajas del multi-stage:**
- Imagen final más pequeña (solo runtime, no SDK)
- Más rápida de descargar y ejecutar
- Más segura (menos superficie de ataque)

### 4.3 Configuración de Producción

**Kestrel (Web Server):**
- Configurado para escuchar en 0.0.0.0:8080
- No usa HTTPS (terminación SSL en reverse proxy)
- Optimizado para contenedores

**Variables de Entorno:**
- Sobrescriben appsettings.json
- Formato: `ConnectionStrings__DefaultConnection`
- Permite configuración dinámica sin recompilar

**Restart Policy:**
- `unless-stopped`: reinicia automáticamente si falla
- Garantiza alta disponibilidad

## 5. SEGURIDAD

### 5.1 Capas de Seguridad

1. **Autenticación:** Verifica identidad (¿quién eres?)
2. **Autorización:** Verifica permisos (¿qué puedes hacer?)
3. **Encriptación:** Protege datos en tránsito y reposo
4. **Validación:** Previene inyección y XSS

### 5.2 Implementaciones Específicas

**Password Hashing:**
- PBKDF2 con salt aleatorio
- Imposible de revertir
- Resistente a rainbow tables

**JWT Security:**
- Firmado con HMAC-SHA256
- Clave secreta de 256 bits mínimo
- Expira después de 7 días
- Validación en cada request

**CORS (Cross-Origin Resource Sharing):**
- Permite que frontends en otros dominios consuman la API
- Configurado para permitir cualquier origen (ajustar en producción)

**SQL Injection Prevention:**
- Entity Framework usa queries parametrizadas
- No concatena strings SQL directamente

**XSS Prevention:**
- Razor escapa HTML automáticamente
- API devuelve JSON (no HTML)

## 6. FLUJOS DE NEGOCIO PRINCIPALES

### 6.1 Registro de Empleado

1. Admin llama a POST /api/Auth/register con datos del empleado
2. Sistema crea un User en ASP.NET Identity con contraseña por defecto
3. Sistema crea un Employee vinculado al User
4. Sistema asigna rol "Employee" al usuario
5. Sistema envía email de bienvenida
6. Empleado puede hacer login y ver su perfil

### 6.2 Consulta de Perfil (Self-Service)

1. Empleado hace login y obtiene JWT
2. Empleado llama a GET /api/Employees/me con el token
3. Sistema extrae el email del token (claim)
4. Sistema busca el Employee por email
5. Sistema devuelve los datos del empleado
6. Empleado puede descargar su CV en PDF

### 6.3 Gestión de Empleados (Admin)

1. Admin hace login y obtiene JWT con rol "Admin"
2. Admin llama a GET /api/Employees
3. Middleware valida que el token tenga rol "Admin"
4. Sistema devuelve lista completa de empleados
5. Admin puede crear, editar o eliminar empleados

## 7. ESCALABILIDAD Y RENDIMIENTO

### 7.1 Optimizaciones Implementadas

**Async/Await:**
- Todas las operaciones de BD son asíncronas
- Libera threads mientras espera I/O
- Permite manejar más requests concurrentes

**Eager Loading:**
```csharp
.Include(e => e.Department)  // Carga Department en la misma query
```
- Evita el problema N+1
- Reduce round-trips a la base de datos

**Connection Pooling:**
- Entity Framework reutiliza conexiones
- Reduce overhead de crear conexiones nuevas

**Stateless API:**
- JWT permite escalado horizontal
- No requiere sticky sessions
- Cada instancia es independiente

### 7.2 Posibles Mejoras Futuras

1. **Caching:** Redis para datos frecuentes
2. **CDN:** Para archivos estáticos
3. **Load Balancer:** Nginx o AWS ALB
4. **Database Replication:** Read replicas para queries
5. **Message Queue:** RabbitMQ para emails asíncronos
6. **Monitoring:** Application Insights o Prometheus

## 8. TESTING Y CALIDAD

### 8.1 Estrategia de Testing (Recomendada)

**Unit Tests:**
- Testear servicios y repositorios
- Mockear DbContext
- Verificar lógica de negocio

**Integration Tests:**
- Testear controllers end-to-end
- Usar base de datos en memoria
- Verificar flujos completos

**API Tests:**
- Postman collections
- Verificar contratos de API
- Testear autenticación y autorización

### 8.2 Logging y Diagnóstico

**Console Logging:**
- Muestra inicio de aplicación
- Confirma migraciones exitosas
- Reporta errores de BD

**Structured Logging (Recomendado):**
- Serilog para logs estructurados
- Enviar a ELK stack o Seq
- Facilita debugging en producción

## 9. DEPLOYMENT Y CI/CD

### 9.1 Proceso de Deployment

**Local Development:**
```bash
dotnet run
```

**Docker Development:**
```bash
docker-compose up --build
```

**Production (Recomendado):**
1. Push a GitHub
2. CI/CD pipeline (GitHub Actions)
3. Build Docker images
4. Push to Docker Hub / AWS ECR
5. Deploy to Kubernetes / AWS ECS
6. Run migrations automáticamente
7. Health checks y rollback si falla

### 9.2 Ambientes

- **Development:** Máquina local, base de datos local
- **Staging:** Docker, base de datos Supabase de prueba
- **Production:** Kubernetes, base de datos Supabase producción

## 10. MANTENIMIENTO Y EVOLUCIÓN

### 10.1 Agregar Nueva Funcionalidad

**Ejemplo: Agregar módulo de Nómina**

1. **Core:** Crear entidad `Payroll`
2. **Infrastructure:** 
   - Crear migración
   - Crear `PayrollRepository`
   - Crear `PayrollService`
3. **Web:** 
   - Crear `PayrollController`
   - Crear vistas Razor
4. **API:** 
   - Crear `PayrollController` API
   - Documentar en Swagger
5. **Testing:** Escribir tests
6. **Deploy:** Migración automática

### 10.2 Buenas Prácticas

- **Migrations:** Nunca editar migraciones aplicadas
- **Breaking Changes:** Versionar la API (v1, v2)
- **Secrets:** Nunca commitear en git
- **Code Review:** Pull requests obligatorios
- **Documentation:** Mantener README actualizado

## 11. CONCLUSIÓN

TalentoPlus es un sistema robusto y moderno que demuestra:
- Arquitectura limpia y mantenible
- Seguridad mediante autenticación y autorización
- Escalabilidad mediante Docker y diseño stateless
- Flexibilidad con múltiples interfaces (Web y API)
- Buenas prácticas de desarrollo .NET

El sistema está listo para producción y puede evolucionar fácilmente para agregar nuevas funcionalidades como gestión de nómina, evaluaciones de desempeño, control de asistencia, etc.
