# 📘 HƯỚNG DẪN THIẾT KẾ DOMAIN ENTITY & CONFIGURATION INFRASTRUCTURE (CLEAN ARCHITECTURE + DDD)

---

# I. NGUYÊN TẮC CỐT LÕI

## 1. Domain KHÔNG phụ thuộc:

- Entity Framework
- Database
- Data Annotation
- Infrastructure

## 2. Infrastructure chịu trách nhiệm:

- Primary Key
- Foreign Key
- Relationship (1-1, 1-N, N-N)
- Index
- Table name
- Column mapping
- Constraint database

---

# II. THIẾT KẾ ENTITY TRONG DOMAIN

## 1. Entity phải bảo vệ invariant

❌ Sai (Anemic Model)

```csharp
public class Course
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public ICollection<Lesson> Lessons { get; set; }
}
```

✔ Đúng (Rich Domain)

```csharp
public class Course
{
    private readonly List<Lesson> _lessons = new();

    public Guid Id { get; private set; }
    public string Title { get; private set; }

    public IReadOnlyCollection<Lesson> Lessons
        => _lessons.AsReadOnly();

    private Course() { } // For ORM

    public Course(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Title is required");

        Id = Guid.NewGuid();
        Title = title;
    }

    public void AddLesson(string lessonTitle)
    {
        if (_lessons.Any(x => x.Title == lessonTitle))
            throw new DomainException("Duplicate lesson");

        _lessons.Add(new Lesson(lessonTitle));
    }
}
```

---

## 2. QUY TẮC QUAN TRỌNG

- Không dùng `public set`
- Không expose `List<>` hoặc `ICollection<>`
- Không dùng `virtual`
- Không dùng `[Key]`, `[ForeignKey]`, `[Index]`
- Không viết Fluent API trong Domain

---

# III. APPLICATION LAYER (USE CASE)

## 1. Dùng DbContext abstraction

```csharp
public interface IAppDbContext
{
    DbSet<Course> Courses { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

## 2. Handler chi orchestration

- Khong dat business logic trong Handler
- Business rule nam trong Domain methods
- Query phuc tap tach ra QueryExtensions

---

## 3. Phân biệt loại rule

### Local Rule (Self rule)

- Validate trong constructor
- Ví dụ: Title không rỗng

### Aggregate Rule (Invariant)

- Không trùng lesson
- Không vượt quá số lượng
- Không modify khi đã publish

Aggregate rule phải đặt trong Aggregate Root.

---

# IV. CONFIGURATION TRONG INFRASTRUCTURE

## 1. DbContext đặt ở Infrastructure

```csharp
public class AppDbContext : DbContext
{
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Lesson> Lessons => Set<Lesson>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
    }
}
```

---

## 2. Mapping bằng IEntityTypeConfiguration

```csharp
public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
               .IsRequired()
               .HasMaxLength(200);

        builder.HasMany<Lesson>("_lessons")
               .WithOne()
               .HasForeignKey("CourseId");

        builder.HasIndex(x => x.Title);
    }
}
```

---

# V. 1-N TRONG DOMAIN VS DATABASE

## Database

- OrderId (FK)
- Table mapping

## Domain

```csharp
private readonly List<Lesson> _lessons;
public IReadOnlyCollection<Lesson> Lessons => _lessons;
```

Domain không biết FK.

---

# VI. CHECKLIST KHI TẠO ENTITY

### ✔ Domain Layer

- [ ] Có private list cho child entity
- [ ] Expose IReadOnlyCollection
- [ ] Có method Add/Remove thay vì expose collection
- [ ] Constructor validate rule nội tại
- [ ] Không có EF attribute
- [ ] Không có navigation property dạng virtual

### ✔ Infrastructure Layer

- [ ] HasKey()
- [ ] HasMany()/WithOne()
- [ ] HasForeignKey()
- [ ] HasIndex()
- [ ] Property().HasMaxLength()
- [ ] Table mapping nếu cần

---

# VII. NGUYÊN TẮC VÀNG

1. Domain model ≠ Database model
2. Aggregate Root kiểm soát toàn bộ state bên trong
3. Infrastructure chỉ là implementation detail
4. Không để ORM quyết định thiết kế Domain

---

# VIII. MẪU FOLDER STRUCTURE

```
/src
  /Domain
    /Entities
    /ValueObjects
    /Exceptions
  /Application
    /Commands
    /Queries
    /Validators
  /Infrastructure
    /Persistence
      AppDbContext.cs
      Configurations/
  /WebAPI
```

---

# IX. KẾT LUẬN

- Domain bảo vệ business logic
- Infrastructure cấu hình database
- Application điều phối use case
- Không trộn layer

Thiết kế đúng từ đầu sẽ tránh 90% bug logic về sau.
