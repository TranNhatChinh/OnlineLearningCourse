# 🏗️ Clean Architecture Template .NET 8

Template dự án ASP.NET Core 8 với Clean Architecture, CQRS, MediatR, và Minimal API.

## 📋 Tổng quan

Template này cung cấp một cấu trúc dự án hoàn chỉnh theo **Clean Architecture** với các patterns và practices hiện đại:

- ✅ **Clean Architecture** - Tách biệt rõ ràng các layers
- ✅ **CQRS Pattern** - Commands và Queries riêng biệt
- ✅ **MediatR** - Request/Response pipeline
- ✅ **Minimal API** - Lightweight endpoints
- ✅ **Mapperly** - Source generator mapping (không runtime overhead)
- ✅ **FluentValidation** - Validation với pipeline behavior
- ✅ **DbContext Abstraction** - `IAppDbContext` for data access
- ✅ **PostgreSQL** - Modern database với Entity Framework Core
- ✅ **Docker Support** - Container-ready

---

## 📁 Cấu trúc Project

```
├── Domain/                          # Core business logic
│   ├── Entities/                   # Domain entities
│   ├── Enums/                      # Domain enumerations
│   ├── Interfaces/                 # Domain contracts (if needed)
│   └── Common/                     # Base entity, domain exceptions
│
├── Application/                    # Use cases & application logic
│   ├── Features/                   # Feature folders (vertical slice)
│   │   └── Products/
│   │       ├── Commands/          # Write operations
│   │       ├── Queries/           # Read operations
│   │       ├── Handlers/          # MediatR handlers
│   │       └── Validations/       # FluentValidation validators
│   ├── Abstractions/               # IAppDbContext, app-level contracts
│   ├── DTOs/                      # Data transfer objects
│   └── Common/
│       ├── CQRS/                  # ICommand, IQuery interfaces
│       ├── Behaviors/             # MediatR pipeline behaviors
│       ├── Exceptions/            # Application exceptions
│       ├── Mappings/              # Mapperly mappers
│       └── Wrappers/              # API response wrappers
│
├── Infrastructure/                 # External concerns
│   ├── Data/                      # DbContext, query extensions
│   └── Migrations/                # EF Core migrations
│
└── Web/                           # API layer
    ├── Endpoints/                 # Minimal API endpoints
    ├── Middleware/                # Global exception handler
    └── Program.cs                 # Application startup
```

---

## 🚀 Getting Started

### Prerequisites

- .NET 8.0 SDK
- PostgreSQL 16+ (hoặc Docker)
- IDE: Visual Studio 2022 / VS Code / Rider

### 1. Clone và Setup

```bash
# Clone template
git clone <repository-url>
cd CleanArchitectureTemplateNet8

# Copy environment file
cp .env.example .env

# Chỉnh sửa .env với thông tin database của bạn
```

### 2. Cấu hình Database

#### Option A: Sử dụng Docker (Recommended)

```bash
# Start PostgreSQL container
docker-compose up -d

# Logs
docker-compose logs -f
```

#### Option B: PostgreSQL local

Tạo database và cập nhật connection string trong [appsettings.json](Web/appsettings.json):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=yourdb;Username=postgres;Password=yourpassword"
  }
}
```

### 3. Run Migrations

```bash
# Từ root folder
dotnet ef database update -s Web/Web.csproj -p Infrastructure/Infrastructure.csproj
```

### 4. Run Application

```bash
# Development
dotnet run --project Web/Web.csproj

# Hoặc với watch
dotnet watch run --project Web/Web.csproj
```

Mở trình duyệt: `https://localhost:7191/swagger`

---

## 🧪 Testing API

### Sử dụng Swagger UI

Navigate to: `https://localhost:7191/swagger`

### Sử dụng Web.http (VS Code)

Cài đặt extension: **REST Client**

Mở file [Web/Web.http](Web/Web.http) và test các endpoints.

### Sample Requests

#### Create Product

```http
POST https://localhost:7191/api/v1/products
Content-Type: application/json

{
  "name": "Laptop Dell XPS 15",
  "description": "High-performance laptop",
  "price": 1999.99,
  "stock": 50,
  "category": "Electronics"
}
```

#### Get All Products

```http
GET https://localhost:7191/api/v1/products?category=Electronics&isActive=true&pageNumber=1&pageSize=10
```

---

## 📦 NuGet Packages

### Domain Layer

- Không có dependencies bên ngoài (pure C#)

### Application Layer

- `MediatR` - CQRS mediator pattern
- `FluentValidation.DependencyInjectionExtensions` - Validation
- `Riok.Mapperly` - Compile-time object mapping

### Infrastructure Layer

- `Npgsql.EntityFrameworkCore.PostgreSQL` - PostgreSQL provider
- `Microsoft.EntityFrameworkCore.Design` - EF Core tools

### Web Layer

- `Microsoft.AspNetCore.OpenApi` - OpenAPI support
- `Swashbuckle.AspNetCore` - Swagger UI

---

## 🏛️ Architecture Principles

### 1. Dependency Rule

```
Web → Application → Domain
     ↘ Infrastructure ↗
```

- **Domain**: Không phụ thuộc vào layer nào
- **Application**: Chỉ phụ thuộc Domain
- **Infrastructure**: Phụ thuộc Domain & Application (implement interfaces)
- **Web**: Phụ thuộc tất cả (composition root)

### 2. CQRS Pattern

**Commands** (Write):

```csharp
public record CreateProductCommand(CreateProductRequest Request) : ICommand<ProductDto>;
```

**Queries** (Read):

```csharp
public record GetAllProductsQuery(...) : IQuery<List<ProductDto>>;
```

### 3. Validation Pipeline

FluentValidation tự động chạy qua `ValidationBehavior` trước khi handler:

```csharp
Request → ValidationBehavior → Handler → Response
```

### 4. Exception Handling

`GlobalExceptionMiddleware` tự động catch và format errors:

- `NotFoundException` → 404
- `ConflictException` → 409
- `ApplicationValidationException` → 400
- `Exception` → 500

### 5. Response Wrapping

Tất cả responses được wrap trong `ApiResponse`:

```json
{
  "succeeded": true,
  "message": "Success",
  "data": { ... },
  "errors": null
}
```

---

## 📚 Pattern Details

### DbContext Abstraction

```csharp
public interface IAppDbContext
{
  DbSet<Product> Products { get; }
  Task<int> SaveChangesAsync(CancellationToken ct);
}
```

### Query Extensions (for complex queries)

```csharp
public static class ProductQueryExtensions
{
  public static IQueryable<Product> IncludeDetails(this IQueryable<Product> query)
    => query.Include(p => p.Category);
}
```

### Domain-Driven Design

Entities có **private setters** và **factory methods**:

```csharp
public class Product : BaseEntity
{
    public string Name { get; private set; }

    private Product() { } // EF Core

    public static Product Create(string name, ...)
    {
        // Validation & business rules
        return new Product { ... };
    }

    public void UpdatePrice(decimal newPrice)
    {
        // Business logic
        Price = newPrice;
        UpdatedAt = DateTime.UtcNow;
    }
}
```

---

## 🔧 Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=cleanarchdb;Username=postgres;Password=yourpass"
  },
  "JwtSettings": {
    "Secret": "your-secret-key-minimum-32-characters",
    "Issuer": "YourApp",
    "Audience": "YourAppUsers",
    "ExpirationMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Environment Variables (.env)

```env
POSTGRES_USER=postgres
POSTGRES_PASSWORD=yourpassword
POSTGRES_DB=cleanarchdb

JWT_SECRET=your-super-secret-key
JWT_ISSUER=YourApp
```

---

## 🎯 How to Add New Feature

👉 **Chi tiết xem file: [API_GENERATION_GUIDE.md](API_GENERATION_GUIDE.md)**

### Quick Steps:

1. **Domain**: Tạo Entity + business rules
2. **Application**:
   - Tạo DTOs
   - Tạo Commands/Queries
   - Implement Handlers
   - Add Validators
   - Create Mapperly Mapper
3. **Infrastructure**:

- Cập nhật DbContext mapping
- Thêm query extensions nếu query phức tạp

4. **Web**: Tạo Endpoints + Register DI
5. **Migration**: `dotnet ef migrations add ...`

---

## 🐳 Docker Support

### Docker Compose

```yaml
services:
  postgres:
    image: postgres:16-alpine
    environment:
      POSTGRES_USER: ${POSTGRES_USER}
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD}
      POSTGRES_DB: ${POSTGRES_DB}
    ports:
      - "5432:5432"
```

### Commands

```bash
# Start services
docker-compose up -d

# View logs
docker-compose logs -f postgres

# Stop services
docker-compose down

# Stop and remove volumes
docker-compose down -v
```

---

## 📖 Best Practices

### ✅ DO:

- Tách biệt Commands và Queries (CQRS)
- Validate ở Application layer (FluentValidation)
- Business logic ở Domain entities (khong trong Handler)
- Sử dụng DTOs cho API responses
- Async/await cho I/O operations
- CancellationToken cho async methods
- Dung `IAppDbContext` (khong dung repository pass-through)
- Dung private list + IReadOnlyCollection cho aggregate

### ❌ DON'T:

- Expose Domain entities trực tiếp trong API
- Business logic trong Controllers/Endpoints
- Validation logic trong Domain entities
- Goi `ApplicationDbContext` truc tiep tu Application layer
- Dat EF attributes trong Domain
- Handler sua property truc tiep neu do la business rule
- Ignore exception handling
- Skip validation

---

## 🔐 Security (TODO)

Template này chưa implement authentication/authorization. Để thêm:

1. **JWT Authentication**
   - Implement trong `Infrastructure/Authentication/`
   - Register trong `Program.cs`

2. **Authorization Policies**
   - Define policies
   - Apply với `.RequireAuthorization()` trên endpoints

3. **User Management**
   - User entity trong Domain
   - Identity integration (optional)

---

## 📊 Performance Tips

1. **Async all the way**: Tất cả I/O operations phải async
2. **DbContext pooling**: Đã enable trong `Program.cs`
3. **Mapperly**: Compile-time mapping (faster than AutoMapper)
4. **Minimal API**: Ít overhead hơn Controllers
5. **Index database**: Thêm indexes cho các queries thường xuyên

---

## 🧹 Code Quality

### Style Guidelines

- Follow Microsoft C# coding conventions
- Use nullable reference types
- Prefer `record` cho DTOs và Commands/Queries
- Use expression-bodied members khi phù hợp

### Tools

- **Analyzers**: Microsoft.CodeAnalysis.NetAnalyzers
- **Formatting**: `.editorconfig`
- **Linting**: SonarLint (recommended)

---

## 🤝 Contributing

Nếu tìm thấy bugs hoặc có suggestions:

1. Fork repository
2. Create feature branch
3. Commit changes
4. Push to branch
5. Open Pull Request

---

## 📝 License

This project is licensed under the MIT License - see LICENSE file for details.

---

## 📞 Support

- 📚 Documentation: [API_GENERATION_GUIDE.md](API_GENERATION_GUIDE.md)
- 🐛 Issues: GitHub Issues
- 💬 Discussions: GitHub Discussions

---

**Built with ❤️ using Clean Architecture principles**
# OnlineLearningCourse
