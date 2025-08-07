# DOTNET_BASE - Clean Architecture Base Project

A .NET 9 base project implementing Clean Architecture with Dapper and PostgreSQL.

## Project Structure

```
DOTNET_BASE/
├── DOTNET_BASE.API/          # Presentation Layer (Web API)
│   └── Controllers/
├── DOTNET_BASE.APPLICATION/  # Application Layer (Services, DTOs)
│   ├── User/                 # User domain (Service, DTO, Interface)
│   └── Account/              # Account domain (Service, DTO, Interface)  
├── DOTNET_BASE.CORE/         # Core Layer (Entities, Interfaces)
│   ├── Entities/             # Domain entities
│   ├── Interfaces/           # Repository interfaces
│   └── Attributes/           # Custom attributes
├── DOTNET_BASE.INFRASTRUCTURE/ # Infrastructure Layer (Data Access)
│   ├── Repositories/         # Repository implementations
│   └── Data/                 # Database context
└── Database/Scripts/         # Database migration scripts
```

## Dependencies

### DOTNET_BASE.API
- References: APPLICATION, INFRASTRUCTURE
- Packages: Swashbuckle.AspNetCore

### DOTNET_BASE.APPLICATION
- References: CORE
- Packages: None (Pure business logic)

### DOTNET_BASE.INFRASTRUCTURE
- References: CORE
- Packages: Dapper, Npgsql

### DOTNET_BASE.CORE
- References: None
- Packages: None (Pure domain logic)

## Database Setup

1. Install PostgreSQL
2. Create database: `dotnet_base_db`
3. Run scripts in order:
   - `Database/Scripts/001_CreateTables.sql`
   - `Database/Scripts/002_SeedData.sql`

## Configuration

Update connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=dotnet_base_db;Username=your_user;Password=your_password;"
  }
}
```

## Sample Entities

- **User**: Basic user management with attributes mapping
- **Account**: Account management with types and balance tracking

## Features

- **Clean Architecture**: Proper separation of concerns
- **Dapper ORM**: Lightweight and fast data access
- **PostgreSQL**: Robust database with BIGSERIAL IDs
- **Attribute Mapping**: Custom `[Table]`, `[Key]`, `[NotMapped]` attributes
- **Folder Organization**: Domain-driven structure in APPLICATION layer
- **Auto Repository**: Base repository pattern with reflection
- **Swagger Documentation**: Built-in API documentation

## API Endpoints

### Users
- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `GET /api/users/by-email/{email}` - Get user by email
- `GET /api/users/by-username/{username}` - Get user by username
- `POST /api/users` - Create user
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user

### Accounts
- `GET /api/accounts` - Get all accounts
- `GET /api/accounts/{id}` - Get account by ID
- `GET /api/accounts/by-type/{type}` - Get accounts by type
- `GET /api/accounts/active` - Get active accounts
- `POST /api/accounts` - Create account
- `PUT /api/accounts/{id}` - Update account
- `DELETE /api/accounts/{id}` - Delete account

## Running the Application

```bash
cd DOTNET_BASE.API
dotnet run
```

Navigate to: https://localhost:5001/swagger

## Adding New Entities

1. **Create Entity**: Add to `DOTNET_BASE.CORE/Entities/` with `[Table("table_name")]` attribute
2. **Create Repository Interface**: Add to `DOTNET_BASE.CORE/Interfaces/`
3. **Implement Repository**: Add to `DOTNET_BASE.INFRASTRUCTURE/Repositories/`
4. **Create Domain Folder**: Add to `DOTNET_BASE.APPLICATION/YourDomain/`
   - `YourDomainDto.cs` (DTOs)
   - `IYourDomainService.cs` (Service interface)
   - `YourDomainService.cs` (Service implementation)
5. **Create Controller**: Add to `DOTNET_BASE.API/Controllers/`
6. **Register Dependencies**: Update `Program.cs`
7. **Database Scripts**: Add to `Database/Scripts/`

## Architecture Benefits

- **Testable**: Clean separation enables easy unit testing
- **Maintainable**: Domain-driven folder structure 
- **Scalable**: Easy to add new domains/entities
- **Database First**: SQL scripts for version control
- **Type Safe**: Strong typing with custom attributes
- **Fast**: Dapper performance with clean abstraction

## Getting Started

1. Clone this repository
2. Setup PostgreSQL database
3. Update connection string in `appsettings.json`
4. Run database scripts in order
5. `dotnet run` in API project
6. Navigate to Swagger UI for testing