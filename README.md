# TaskMgmt API (.NET 8 + EF Core + SQL Server)

JWT-secured Task & Project management API.

## Tech
- ASP.NET Core (.NET 8), Controllers
- EF Core + SQL Server (migrations)
- JWT Bearer Auth (roles: Manager, Employee)
- FluentValidation, AutoMapper
- Swagger/OpenAPI
- Quartz job @ 9AM for overdue tasks (email)
- Image upload for tasks (stores URL)

## Run locally

1. **DB** (choose one)
    - **LocalDB** (Visual Studio):
        - Connection string in `appsettings.json`:
          ```
          Server=(localdb)\\MSSQLLocalDB;Database=TaskMgmtDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True
          ```
    - **Docker SQL Server**:
      ```
      docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrong!Passw0rd" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
      ```
      Connection string:
      ```
      Server=localhost,1433;Database=TaskMgmtDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;
      ```

2. **Migrate & seed**
dotnet ef database update


3. **Run**
   Swagger: `http://localhost:<port>/swagger`

## Test credentials
- Manager: `manager / Pass@123`
- Employee: `employee / Pass@123`

## Auth
- `POST /api/auth/login` → `{ token, userId, username, name, role }`
- `POST /api/auth/register` → creates Employee by default

## Users (Manager only)
- `GET /api/users`
- `GET /api/users/{id}`
- `POST /api/users`
- `PUT /api/users/{id}`
- `DELETE /api/users/{id}`

## Projects
- `GET /api/projects`
- `GET /api/projects/{id}`
- `POST /api/projects`
- `PUT /api/projects/{id}`
- `DELETE /api/projects/{id}` (Manager only)

## Tasks
- `GET /api/tasks?status=InProgress&priority=High&projectId=1&assignedToUserId=2&search=readme&page=1&pageSize=20`
- `GET /api/tasks/{id}`
- `POST /api/tasks`
- `PUT /api/tasks/{id}`
- `DELETE /api/tasks/{id}`
- `POST /api/tasks/{id}/image` (multipart/form-data, field name `file`) → sets `ImageUrl`

## Reports
- `GET /api/reports/tasks/summary` → counts by status, priority, and overdue

## Background Job
Daily at **09:00** UTC, finds tasks with `DueDate < now` and `Status != Done`, and emails the assignee.  
Configure SMTP in `appsettings.json`:
```json
"Email": {
"From": "no-reply@example.com",
"Smtp": { "Host": "", "Port": "587", "User": "", "Pass": "", "Ssl": "true" }
}
```

---

 Migrations to run

You’ve already created `InitialCreate`. After adding `ImageUrl`, run:

---

 Quick cURL smoke tests

```bash
# Login
curl -s -X POST http://localhost:5249/api/auth/login \
 -H "Content-Type: application/json" \
 -d '{"username":"manager","password":"Pass@123"}'

# List projects (replace TOKEN)
curl -H "Authorization: Bearer TOKEN" http://localhost:5249/api/projects

# Create task
curl -s -X POST http://localhost:5249/api/tasks \
 -H "Authorization: Bearer TOKEN" -H "Content-Type: application/json" \
 -d '{"title":"Spec doc","priority":1,"projectId":1,"assignedToUserId":2}'

# Upload image (Windows PowerShell)
curl.exe -X POST "http://localhost:5249/api/tasks/1/image" ^
 -H "Authorization: Bearer TOKEN" ^
 -F "file=@C:\path\to\image.png"
```
## Repo Structure
- Controllers/         → API endpoints
- Data/                → EF Core DbContext, seeding
- Domain/Entities      → User, Project, TaskItem
- Domain/Enums         → TaskStatus, TaskPriority
- Dtos/                → Request/Response DTOs
- Jobs/                → Quartz scheduled job
- Middleware/          → Error handling
- Services/            → JWT, Email, Password hashing
- wwwroot/uploads      → image uploads
