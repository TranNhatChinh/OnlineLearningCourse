# 📘 Hướng dẫn Generate API với Clean Architecture Template

## 🎯 Tổng quan

Template này sử dụng **Clean Architecture** với **Minimal API**, **CQRS Pattern**, **MediatR**, và **Mapperly**. Tài liệu này hướng dẫn chi tiết cách tạo mới một API endpoint hoàn chỉnh theo đúng cấu trúc.

---

## 📁 Cấu trúc Layers

```
├── Domain/              # Core business logic, entities
├── Application/         # Use cases, DTOs, CQRS handlers
├── Infrastructure/      # Database, EF Core mapping, query extensions
└── Web/                 # API endpoints, middleware
```

---

## 🔄 Quy trình tạo mới một API Feature

### **Ví dụ: Tạo API cho entity `Order`**

---

## 🧭 Cấu trúc thư mục cho feature

```
Application/
    Features/
        Orders/
            Commands/
            Queries/
            Handlers/
            Validators/
    Abstractions/
        IAppDbContext.cs
Infrastructure/
    Data/
        ApplicationDbContext.cs
        QueryExtensions/
```

## 1️⃣ DOMAIN LAYER

### 📍 Vị trí: `Domain/Entities/`

**Tạo entity domain model:**

```csharp
// Domain/Entities/Order.cs
using Domain.Common;

namespace Domain.Entities;

public class Order : BaseEntity
{
    // Private setters - enforce encapsulation
    public string OrderNumber { get; private set; } = string.Empty;
    public decimal TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime OrderDate { get; private set; }
    public Guid CustomerId { get; private set; }

    // Navigation properties (DDD style)
    private readonly List<OrderItem> _orderItems = new();
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    // Private constructor - force domain factory
    private Order() { }

    // ✅ Domain factory method - duy nhất cách tạo entity
    public static Order Create(
        string orderNumber,
        Guid customerId,
        decimal totalAmount)
    {
        // Validation logic
        if (string.IsNullOrWhiteSpace(orderNumber))
            throw new ArgumentException("Order number cannot be empty");

        if (totalAmount <= 0)
            throw new ArgumentException("Total amount must be greater than 0");

        return new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = orderNumber,
            CustomerId = customerId,
            TotalAmount = totalAmount,
            Status = OrderStatus.Pending,
            OrderDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
    }

    // ✅ Domain methods for business logic
    public void UpdateStatus(OrderStatus newStatus)
    {
        if (Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Cannot update cancelled order");

        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTotalAmount(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive");

        TotalAmount = amount;
        UpdatedAt = DateTime.UtcNow;
    }
}
```

**Luu y Domain:**

- Khong dung EF attributes trong Domain (`[Key]`, `[ForeignKey]`, `[Index]`)
- Khong expose `public List<T>`; dung `private List<T>` + `IReadOnlyCollection<T>`

### 📍 Vị trí: `Domain/Enums/` (nếu cần)

```csharp
// Domain/Enums/OrderStatus.cs
namespace Domain.Enums;

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Shipped = 2,
    Delivered = 3,
    Cancelled = 4
}
```

### 📍 Vị trí: `Application/Abstractions/`

**Dùng DbContext thông qua abstraction (không dùng Repository nếu chỉ pass-through):**

```csharp
// Application/Abstractions/IAppDbContext.cs
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions;

public interface IAppDbContext
{
    DbSet<Order> Orders { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

---

## 2️⃣ INFRASTRUCTURE LAYER

### 📍 Vị trí: `Infrastructure/Data/ApplicationDbContext.cs`

**Implement `IAppDbContext`:**

```csharp
using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbContext : DbContext, IAppDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
}
```

### 📍 Vị trí: `Infrastructure/Data/QueryExtensions/`

**Tách query phức tạp khỏi handler để gọn code:**

```csharp
// Infrastructure/Data/QueryExtensions/OrderQueryExtensions.cs
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.QueryExtensions;

public static class OrderQueryExtensions
{
    public static IQueryable<Order> IncludeDetails(this IQueryable<Order> query)
        => query.Include(o => o.OrderItems);
}
```

---

## 3️⃣ APPLICATION LAYER

### 📍 Vị trí: `Application/DTOs/`

**Tạo DTO:**

```csharp
// Application/DTOs/OrderDto.cs
namespace Application.DTOs;

public class OrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// Application/DTOs/CreateOrderRequest.cs
public record CreateOrderRequest(
    string OrderNumber,
    Guid CustomerId,
    decimal TotalAmount
);

// Application/DTOs/UpdateOrderRequest.cs
public record UpdateOrderRequest(
    string OrderNumber,
    decimal TotalAmount,
    string Status
);
```

### 📍 Vị trí: `Application/Common/Mappings/`

**Tạo Mapperly mapper:**

```csharp
// Application/Common/Mappings/OrderMapper.cs
using Application.DTOs;
using Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Application.Common.Mappings;

[Mapper]
public partial class OrderMapper
{
    public partial OrderDto ToDto(Order order);
    public partial List<OrderDto> ToDtoList(List<Order> orders);
}
```

### 📍 Vị trí: `Application/Features/Orders/Commands/`

**Tạo Commands:**

```csharp
// Application/Features/Orders/Commands/CreateOrderCommand.cs
using Application.Common.CQRS;
using Application.DTOs;

namespace Application.Features.Orders.Commands;

public record CreateOrderCommand(CreateOrderRequest Request) : ICommand<OrderDto>;

// Application/Features/Orders/Commands/UpdateOrderCommand.cs
public record UpdateOrderCommand(Guid Id, UpdateOrderRequest Request) : ICommand<OrderDto>;

// Application/Features/Orders/Commands/DeleteOrderCommand.cs
public record DeleteOrderCommand(Guid Id) : ICommand;
```

### 📍 Vị trí: `Application/Features/Orders/Queries/`

**Tạo Queries:**

```csharp
// Application/Features/Orders/Queries/GetAllOrdersQuery.cs
using Application.Common.CQRS;
using Application.DTOs;

namespace Application.Features.Orders.Queries;

public record GetAllOrdersQuery(
    Guid? CustomerId = null,
    string? Status = null,
    int PageNumber = 1,
    int PageSize = 10
) : IQuery<List<OrderDto>>;

// Application/Features/Orders/Queries/GetOrderByIdQuery.cs
public record GetOrderByIdQuery(Guid Id) : IQuery<OrderDto>;
```

### 📍 Vị trí: `Application/Features/Orders/Handlers/`

**Tạo Command Handlers:**

```csharp
// Application/Features/Orders/Handlers/CreateOrderCommandHandler.cs
using Application.Abstractions;
using Application.Common.CQRS;
using Application.Common.Exceptions;
using Application.Common.Mappings;
using Application.DTOs;
using Application.Features.Orders.Commands;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Orders.Handlers;

public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, OrderDto>
{
    private readonly IAppDbContext _context;
    private readonly OrderMapper _mapper;

    public CreateOrderCommandHandler(IAppDbContext context, OrderMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Kiểm tra trùng OrderNumber
        var existing = await _context.Orders
            .FirstOrDefaultAsync(o => o.OrderNumber == request.Request.OrderNumber, cancellationToken);

        if (existing != null)
            throw new ConflictException("Order", "OrderNumber", request.Request.OrderNumber);

        // Tạo entity qua domain factory
        var order = Order.Create(
            request.Request.OrderNumber,
            request.Request.CustomerId,
            request.Request.TotalAmount
        );

        await _context.Orders.AddAsync(order, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.ToDto(order);
    }
}
```

```csharp
// Application/Features/Orders/Handlers/UpdateOrderCommandHandler.cs
using Application.Abstractions;
using Application.Common.CQRS;
using Application.Common.Exceptions;
using Application.Common.Mappings;
using Application.DTOs;
using Application.Features.Orders.Commands;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Orders.Handlers;

public class UpdateOrderCommandHandler : ICommandHandler<UpdateOrderCommand, OrderDto>
{
    private readonly IAppDbContext _context;
    private readonly OrderMapper _mapper;

    public UpdateOrderCommandHandler(IAppDbContext context, OrderMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<OrderDto> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (order == null)
            throw new NotFoundException(nameof(Order), request.Id);

        // Sử dụng domain methods để update
        if (Enum.TryParse<OrderStatus>(request.Request.Status, out var status))
        {
            order.UpdateStatus(status);
        }

        order.UpdateTotalAmount(request.Request.TotalAmount);

        _context.Orders.Update(order);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.ToDto(order);
    }
}
```

```csharp
// Application/Features/Orders/Handlers/DeleteOrderCommandHandler.cs
using Application.Abstractions;
using Application.Common.CQRS;
using Application.Common.Exceptions;
using Application.Features.Orders.Commands;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Orders.Handlers;

public class DeleteOrderCommandHandler : ICommandHandler<DeleteOrderCommand>
{
    private readonly IAppDbContext _context;

    public DeleteOrderCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (order == null)
            throw new NotFoundException(nameof(Order), request.Id);

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
```

**Tạo Query Handlers:**

```csharp
// Application/Features/Orders/Handlers/GetAllOrdersQueryHandler.cs
using Application.Abstractions;
using Application.Common.CQRS;
using Application.Common.Mappings;
using Application.DTOs;
using Application.Features.Orders.Queries;
using Infrastructure.Data.QueryExtensions;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Orders.Handlers;

public class GetAllOrdersQueryHandler : IQueryHandler<GetAllOrdersQuery, List<OrderDto>>
{
    private readonly IAppDbContext _context;
    private readonly OrderMapper _mapper;

    public GetAllOrdersQueryHandler(IAppDbContext context, OrderMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<OrderDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Orders.AsQueryable().IncludeDetails();

        if (request.CustomerId.HasValue)
        {
            query = query.Where(o => o.CustomerId == request.CustomerId.Value);
        }

        var orders = await query.ToListAsync(cancellationToken);

        // Apply filtering if needed
        if (!string.IsNullOrEmpty(request.Status))
        {
            orders = orders.Where(o => o.Status.ToString() == request.Status).ToList();
        }

        return _mapper.ToDtoList(orders);
    }
}
```

```csharp
// Application/Features/Orders/Handlers/GetOrderByIdQueryHandler.cs
using Application.Abstractions;
using Application.Common.CQRS;
using Application.Common.Exceptions;
using Application.Common.Mappings;
using Application.DTOs;
using Application.Features.Orders.Queries;
using Domain.Entities;
using Infrastructure.Data.QueryExtensions;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Orders.Handlers;

public class GetOrderByIdQueryHandler : IQueryHandler<GetOrderByIdQuery, OrderDto>
{
    private readonly IAppDbContext _context;
    private readonly OrderMapper _mapper;

    public GetOrderByIdQueryHandler(IAppDbContext context, OrderMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .IncludeDetails()
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (order == null)
            throw new NotFoundException(nameof(Order), request.Id);

        return _mapper.ToDto(order);
    }
}
```

### 📍 Vị trí: `Application/Features/Orders/Validations/`

**Tạo Validators:**

```csharp
// Application/Features/Orders/Validations/CreateOrderCommandValidator.cs
using Application.Features.Orders.Commands;
using FluentValidation;

namespace Application.Features.Orders.Validations;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.Request.OrderNumber)
            .NotEmpty().WithMessage("Order number is required")
            .MaximumLength(50).WithMessage("Order number cannot exceed 50 characters");

        RuleFor(x => x.Request.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required");

        RuleFor(x => x.Request.TotalAmount)
            .GreaterThan(0).WithMessage("Total amount must be greater than 0");
    }
}

// Application/Features/Orders/Validations/UpdateOrderCommandValidator.cs
public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Order ID is required");

        RuleFor(x => x.Request.OrderNumber)
            .NotEmpty().WithMessage("Order number is required")
            .MaximumLength(50).WithMessage("Order number cannot exceed 50 characters");

        RuleFor(x => x.Request.TotalAmount)
            .GreaterThan(0).WithMessage("Total amount must be greater than 0");

        RuleFor(x => x.Request.Status)
            .NotEmpty().WithMessage("Status is required")
            .Must(s => Enum.TryParse<OrderStatus>(s, out _))
            .WithMessage("Invalid order status");
    }
}
```

---

## 4️⃣ WEB LAYER (API)

### 📍 Vị trí: `Web/Endpoints/`

**Tạo Minimal API Endpoints:**

```csharp
// Web/Endpoints/OrderEndpoints.cs
using Application.Common.Wrappers;
using Application.DTOs;
using Application.Features.Orders.Commands;
using Application.Features.Orders.Queries;
using MediatR;

namespace Web.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/orders")
            .WithTags("Orders");

        group.MapGetAllOrders();
        group.MapGetOrderById();
        group.MapCreateOrder();
        group.MapUpdateOrder();
        group.MapDeleteOrder();

        return app;
    }

    /// <summary>
    /// GET /api/v1/orders?customerId=&status=&pageNumber=&pageSize=
    /// </summary>
    private static void MapGetAllOrders(this IEndpointRouteBuilder group)
    {
        group.MapGet("/", async (
            ISender sender,
            Guid? customerId,
            string? status,
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken ct = default) =>
        {
            var query = new GetAllOrdersQuery(customerId, status, pageNumber, pageSize);
            var result = await sender.Send(query, ct);
            return Results.Ok(ApiResponse<List<OrderDto>>.Ok(result));
        })
        .WithName("GetAllOrders")
        .WithSummary("Get all orders with filtering & pagination")
        .AllowAnonymous();
    }

    /// <summary>
    /// GET /api/v1/orders/{id}
    /// </summary>
    private static void MapGetOrderById(this IEndpointRouteBuilder group)
    {
        group.MapGet("/{id:guid}", async (
            ISender sender,
            Guid id,
            CancellationToken ct) =>
        {
            var query = new GetOrderByIdQuery(id);
            var result = await sender.Send(query, ct);
            return Results.Ok(ApiResponse<OrderDto>.Ok(result));
        })
        .WithName("GetOrderById")
        .WithSummary("Get order by ID")
        .AllowAnonymous();
    }

    /// <summary>
    /// POST /api/v1/orders
    /// </summary>
    private static void MapCreateOrder(this IEndpointRouteBuilder group)
    {
        group.MapPost("/", async (
            ISender sender,
            CreateOrderRequest request,
            CancellationToken ct) =>
        {
            var command = new CreateOrderCommand(request);
            var result = await sender.Send(command, ct);
            return Results.Created($"/api/v1/orders/{result.Id}",
                ApiResponse<OrderDto>.Ok(result, "Order created successfully"));
        })
        .WithName("CreateOrder")
        .WithSummary("Create a new order");
    }

    /// <summary>
    /// PUT /api/v1/orders/{id}
    /// </summary>
    private static void MapUpdateOrder(this IEndpointRouteBuilder group)
    {
        group.MapPut("/{id:guid}", async (
            ISender sender,
            Guid id,
            UpdateOrderRequest request,
            CancellationToken ct) =>
        {
            var command = new UpdateOrderCommand(id, request);
            var result = await sender.Send(command, ct);
            return Results.Ok(ApiResponse<OrderDto>.Ok(result, "Order updated successfully"));
        })
        .WithName("UpdateOrder")
        .WithSummary("Update order by ID");
    }

    /// <summary>
    /// DELETE /api/v1/orders/{id}
    /// </summary>
    private static void MapDeleteOrder(this IEndpointRouteBuilder group)
    {
        group.MapDelete("/{id:guid}", async (
            ISender sender,
            Guid id,
            CancellationToken ct) =>
        {
            var command = new DeleteOrderCommand(id);
            await sender.Send(command, ct);
            return Results.Ok(ApiResponse.Ok("Order deleted successfully"));
        })
        .WithName("DeleteOrder")
        .WithSummary("Delete order by ID");
    }
}
```

### 📍 Cập nhật: `Web/Program.cs`

```csharp
// Register DbContext abstraction
builder.Services.AddScoped<IAppDbContext, ApplicationDbContext>();

// Register new Mapper
builder.Services.AddSingleton<OrderMapper>();

// ... existing code ...

// Map Order Endpoints
app.MapOrderEndpoints();
```

---

## 5️⃣ DATABASE MIGRATION

### Chạy lệnh migration:

```bash
# Từ thư mục Infrastructure
dotnet ef migrations add AddOrderEntity -s ../Web/Web.csproj

# Apply migration
dotnet ef database update -s ../Web/Web.csproj
```

---

## 6️⃣ TESTING (Optional - Web.http)

**Thêm vào `Web/Web.http`:**

```http
### Get All Orders
GET {{Web_HostAddress}}/api/v1/orders
Accept: application/json

### Get Order By Id
GET {{Web_HostAddress}}/api/v1/orders/{{orderId}}
Accept: application/json

### Create Order
POST {{Web_HostAddress}}/api/v1/orders
Content-Type: application/json

{
  "orderNumber": "ORD-2024-001",
  "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "totalAmount": 299.99
}

### Update Order
PUT {{Web_HostAddress}}/api/v1/orders/{{orderId}}
Content-Type: application/json

{
  "orderNumber": "ORD-2024-001-UPDATED",
  "totalAmount": 349.99,
  "status": "Confirmed"
}

### Delete Order
DELETE {{Web_HostAddress}}/api/v1/orders/{{orderId}}
```

---

## ✅ CHECKLIST - Hoàn thiện một Feature

- [ ] **Domain Layer**
  - [ ] Tạo Entity trong `Domain/Entities/`
  - [ ] Tạo Enum (nếu cần) trong `Domain/Enums/`

- [ ] **Infrastructure Layer**
  - [ ] Thêm `DbSet` vào `ApplicationDbContext`
    - [ ] Mapping cấu hình (Fluent API) nếu cần
    - [ ] Tạo query extensions cho query phức tạp

- [ ] **Application Layer**
  - [ ] Cập nhật `IAppDbContext` nếu có DbSet mới
  - [ ] Tạo DTOs trong `Application/DTOs/`
  - [ ] Tạo Mapperly Mapper trong `Application/Common/Mappings/`
  - [ ] Tạo Commands trong `Application/Features/{Entity}/Commands/`
  - [ ] Tạo Queries trong `Application/Features/{Entity}/Queries/`
  - [ ] Tạo Command Handlers trong `Application/Features/{Entity}/Handlers/`
  - [ ] Tạo Query Handlers trong `Application/Features/{Entity}/Handlers/`
  - [ ] Tạo Validators trong `Application/Features/{Entity}/Validations/`

- [ ] **Web Layer**
  - [ ] Tạo Endpoints file trong `Web/Endpoints/`
    - [ ] Register `IAppDbContext` và Mapper trong `Program.cs`
  - [ ] Map Endpoints trong `Program.cs`

- [ ] **Database**
  - [ ] Chạy EF Migration
  - [ ] Apply Database Update

- [ ] **Testing**
  - [ ] Thêm test cases vào `Web.http`
  - [ ] Test tất cả endpoints

---

## 🎨 Best Practices

### 1. **Naming Conventions**

- Entity: `Order`, `Product`, `Customer`
- DTO: `OrderDto`, `CreateOrderRequest`, `UpdateOrderRequest`
- Command: `CreateOrderCommand`, `UpdateOrderCommand`
- Query: `GetAllOrdersQuery`, `GetOrderByIdQuery`
- Handler: `CreateOrderCommandHandler`, `GetOrderByIdQueryHandler`
- Validator: `CreateOrderCommandValidator`
- Endpoints: `OrderEndpoints`

### 2. **CQRS Pattern**

- **Commands**: Modify data (Create, Update, Delete)
- **Queries**: Read data (Get, List)
- Luôn trả về DTO, không expose entity

### 3. **Validation**

- FluentValidation tự động chạy qua `ValidationBehavior`
- Validate input ở Application layer
- Business logic validation ở Domain layer

### 4. **Exception Handling**

- `NotFoundException`: Entity không tồn tại
- `ConflictException`: Conflict (duplicate key, etc.)
- `ApplicationValidationException`: Validation errors
- `GlobalExceptionMiddleware` tự động xử lý

### 5. **Mapping**

- Sử dụng **Mapperly** (source generator) thay vì AutoMapper
- Mapper phải được register trong DI container
- Luôn map từ Entity → DTO

### 6. **DbContext Abstraction**

- Dùng `IAppDbContext` để Application không phụ thuộc Infrastructure
- Không dùng Repository nếu chỉ pass-through
- Query phức tạp nên tách ra `Infrastructure/Data/QueryExtensions`

### 7. **Handlers**

- Handler chỉ orchestration, **không chứa business logic**
- Không sửa property trực tiếp nếu đó là business rule
- Business rule phải nằm trong Domain methods

### 8. **Domain Encapsulation (No public List<T>)**

Sai:

```csharp
public class Order
{
    public List<OrderItem> Items { get; set; } = new();
}
```

Vấn đề:

```csharp
order.Items.Clear();
order.Items.Add(null);
order.Items.Add(new OrderItem(...));
```

Đúng:

```csharp
public class Order
{
    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public void AddItem(Guid productId, int quantity, decimal price)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be > 0");

        var item = new OrderItem(productId, quantity, price);
        _items.Add(item);
    }

    public void RemoveItem(Guid itemId)
    {
        var item = _items.FirstOrDefault(x => x.Id == itemId);
        if (item == null)
            throw new DomainException("Item not found");

        _items.Remove(item);
    }
}
```

---

## 🔧 Cấu hình cần thiết

### **appsettings.json**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=cleanarchdb;Username=postgres;Password=yourpassword"
  },
  "JwtSettings": {
    "Secret": "your-secret-key-at-least-32-characters",
    "Issuer": "CleanArchTemplate",
    "Audience": "CleanArchTemplateUsers",
    "ExpirationMinutes": 60
  }
}
```

### **.env** (for Docker)

```env
POSTGRES_USER=postgres
POSTGRES_PASSWORD=yourpassword
POSTGRES_DB=cleanarchdb
```

---

## 📚 Dependencies chính

```xml
<!-- Application Layer -->
<PackageReference Include="FluentValidation.DependencyInjectionExtensions" />
<PackageReference Include="MediatR" />
<PackageReference Include="Riok.Mapperly" />

<!-- Infrastructure Layer -->
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" />

<!-- Web Layer -->
<PackageReference Include="Microsoft.AspNetCore.OpenApi" />
<PackageReference Include="Swashbuckle.AspNetCore" />
```

---

## 🚀 Khởi chạy Project

```bash
# Build solution
dotnet build

# Run migrations
dotnet ef database update -s Web/Web.csproj -p Infrastructure/Infrastructure.csproj

# Run application
dotnet run --project Web/Web.csproj

# Hoặc sử dụng Docker
docker-compose up -d
```

---

## 📞 Notes

- Template này follow **Vertical Slice Architecture** kết hợp **Clean Architecture**
- **CQRS** với **MediatR** để tách biệt read/write operations
- **FluentValidation** với **ValidationBehavior** tự động validate
- **Mapperly** (compile-time) thay vì AutoMapper (runtime)
- **Minimal API** thay vì Controllers
- **IAppDbContext** cho data access (khong dung repository pass-through)

---

## 🎯 Ví dụ nhanh: Tạo API mới trong 5 phút

1. Copy folder `Application/Features/Products/` → rename thành entity mới
2. Tạo Entity trong `Domain/`
3. Cap nhat `IAppDbContext` neu can
4. Copy `ProductEndpoints.cs` → rename và update
5. Register trong `Program.cs`
6. Run migration
7. Test!

---

**Happy Coding! 🎉**
