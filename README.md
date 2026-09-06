# 💅 Nailify - Advanced Nail Virtual Try-On and Booking System
### *Hệ thống đặt lịch & công cụ thử móng thông minh*

[![CI/CD - Build, Test & Deploy to Render](https://github.com/NailifyCapstone-SU26SE23/Nailify-BE/actions/workflows/deploy-render.yml/badge.svg)](https://github.com/NailifyCapstone-SU26SE23/Nailify-BE/actions/workflows/deploy-render.yml)
![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Containerized-2496ED?style=flat&logo=docker&logoColor=white)
![Render](https://img.shields.io/badge/Render-Deployed-46E3B7?style=flat&logo=render&logoColor=black)
![Clean Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-0078D4?style=flat)
![Unit Tests](https://img.shields.io/badge/Unit%20Tests-180%2B%20Passed-brightgreen?style=flat)

---

## 🏗️ 1. Kiến trúc Hệ thống (Technical Architecture)

Dự án Backend sử dụng nguyên lý **Clean Architecture** kết hợp với **Domain-Driven Design (DDD)** đảm bảo tính mở rộng, bảo mật và dễ bảo trì:

```
          ┌─────────────────────────────────────────┐
          │      Nailify.Capstone.Presentation      │  (ASP.NET Core Web API / Controllers)
          └────────────────────┬────────────────────┘
                               │
          ┌────────────────────▼────────────────────┐
          │      Nailify.Capstone.Application       │  (DTOs, Services, Interfaces, Helpers)
          └──────────┬───────────────────┬──────────┘
                     │                   │
  ┌──────────────────▼──────┐   ┌────────▼────────────────┐
  │  Nailify.Capstone.Domain │   │ Nailify.Capstone.Infrast│  (EF Core, DbContext, SignalR,
  │  (Entities, Enums,      │   │  Repositories, Cloud)   │   Cloudinary, MailKit, JWT)
  │   Domain Events)        │   └─────────────────────────┘
  └─────────────────────────┘
```

---

## ⚙️ 2. Tech Stack & Công nghệ tích hợp

- **Core Framework**: .NET 8.0 Web API (C#)
- **Database**: Entity Framework Core 8.0, PostgreSQL
- **Real-time Engine**: ASP.NET Core SignalR (Thông báo & Cập nhật trạng thái đặt lịch)
- **Security & Auth**: JWT (JSON Web Tokens), BCrypt Password Hashing, Role-based Access Control (RBAC)
- **Cloud & Media**: Cloudinary API
- **Notification & Mail**: MailKit & MimeKit (Gửi Email xác nhận & mã OTP)
- **Containerization**: Docker (Multi-stage build)
- **CI/CD Pipeline**: GitHub Actions + Render Deploy Hook
- **Testing**: xUnit, Moq, FluentAssertions

---


## 🛠️ 3. Cài đặt & Chạy cục bộ (Local Setup Guide)

### Yêu cầu tiên quyết (Prerequisites)
- SDK: [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Database: PostgreSQL
- Tools: Visual Studio 2022 / VS Code / JetBrains Rider

### Các bước khởi chạy

1. **Clone repository**:
   ```bash
   git clone https://github.com/NailifyCapstone-SU26SE23/Nailify-BE.git
   cd Nailify-BE
   ```

2. **Khôi phục dependencies**:
   ```bash
   dotnet restore
   ```

3. **Cấu hình Connection String**:
   Cập nhật chuỗi kết nối PostgreSQL trong `Nailify.Capstone.Presentation/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER;Database=NailifyDb;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```

4. **Khởi chạy API Backend & Auto-Migration**:
   ```bash
   dotnet run --project Nailify.Capstone.Presentation
   ```
   - Truy cập Swagger UI tại: `http://localhost:5004/swagger` hoặc `https://localhost:7066/swagger`.
   > 💡 **Lưu ý**: Khi ứng dụng khởi chạy (`dotnet run`), hệ thống sẽ **tự động chạy Migration** (`app.ApplyMigrations()`) để tạo/cập nhật bảng trong Database PostgreSQL cho bạn.
   >
   > *(Tùy chọn thủ công)* Nếu bạn muốn áp dụng Migration riêng trước khi khởi chạy app:
   > ```bash
   > dotnet ef database update --project Nailify.Capstone.Infrastructure --startup-project Nailify.Capstone.Presentation
   > ```

5. **Chạy bộ Unit Tests**:
   ```bash
   dotnet test
   ```

---

## 🐳 4. Khởi chạy bằng Docker

Build và chạy Container trực tiếp bằng Docker:

```bash
# Build Docker Image
docker build -t nailify-be .

# Run Docker Container
docker run -d -p 8080:8080 --name nailify_app nailify-be
```

---

## 👥 5. Đội ngũ phát triển (Development Team)

- **Capstone Project**: Nailify - Advanced Nail Virtual Try-On and Booking System
- **Repository**: [Nailify-BE GitHub](https://github.com/NailifyCapstone-SU26SE23/Nailify-BE)
