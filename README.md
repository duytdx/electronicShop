# ElectronyatShop - Cửa hàng thiết bị điện tử

## 📋 Giới thiệu

ElectronyatShop là một ứng dụng web thương mại điện tử được xây dựng bằng ASP.NET Core 8.0, chuyên bán các sản phẩm điện tử như laptop, tai nghe và các thiết bị công nghệ khác. Dự án sử dụng Entity Framework Core với MySQL làm cơ sở dữ liệu và hỗ trợ triển khai bằng Docker.

## 🛠️ Công nghệ sử dụng

- **Framework**: ASP.NET Core 8.0
- **Database**: MySQL / SQLite / SQL Server
- **ORM**: Entity Framework Core 8.0
- **Authentication**: ASP.NET Core Identity
- **Frontend**: MVC Pattern với Razor Views
- **Containerization**: Docker
- **Web Server**: Nginx (cho production)

## 📦 Tính năng chính

- ✅ Quản lý sản phẩm (CRUD)
- ✅ Giỏ hàng và thanh toán
- ✅ Quản lý đơn hàng
- ✅ Xác thực và phân quyền người dùng
- ✅ Giao diện quản trị (Admin)
- ✅ Responsive design
- ✅ Hỗ trợ nhiều loại cơ sở dữ liệu

## 🚀 Hướng dẫn cài đặt

### Yêu cầu hệ thống

- .NET 8.0 SDK
- MySQL Server (hoặc SQL Server/SQLite)
- Docker (tùy chọn)
- Visual Studio 2022 hoặc VS Code

### Cài đặt local

1. **Clone repository**

   ```bash
   git clone https://github.com/duytdx/ElectronyatShop.git
   cd ElectronyatShop
   ```

2. **Cấu hình cơ sở dữ liệu**
   
   Chỉnh sửa file `appsettings.json`:

   ```json
   {
     "ConnectionStrings": {
       "MySqlConnection": "Server=localhost;Port=3306;Database=ElectronyatShop;User=root;Password=your_password;"
     }
   }
   ```

3. **Restore packages**

   ```bash
   cd ElectronyatShop
   dotnet restore
   ```

4. **Chạy migrations**

   ```bash
   dotnet ef database update
   ```

5. **Chạy ứng dụng**

   ```bash
   dotnet run
   ```

6. **Truy cập ứng dụng**
   
   Mở trình duyệt và truy cập: `https://localhost:7000`

### Cài đặt bằng Docker

1. **Chạy với Docker Compose**

   ```bash
   docker-compose up -d
   ```

2. **Truy cập ứng dụng**
   
   Ứng dụng sẽ chạy trên port được cấu hình trong docker-compose.yaml

## 📁 Cấu trúc dự án

```text
ElectronyatShop/
├── Controllers/           # Các controller xử lý logic
│   ├── AdminController.cs
│   ├── CartController.cs
│   ├── OrderController.cs
│   └── ProductController.cs
├── Data/                  # Cấu hình database và migrations
│   ├── ElectronyatShopDbContext.cs
│   ├── Extensions.cs
│   └── Scripts/
├── Models/                # Các model entities
│   ├── ApplicationUser.cs
│   ├── Cart.cs
│   ├── Order.cs
│   └── Product.cs
├── ViewModels/            # View models cho MVC
├── Views/                 # Razor views
│   ├── Admin/
│   ├── Cart/
│   ├── Order/
│   ├── Product/
│   └── Shared/
├── wwwroot/               # Static files (CSS, JS, images)
└── Areas/Identity/        # Identity UI pages
```

## 🔧 Cấu hình

### Database Connection

Ứng dụng hỗ trợ nhiều loại database. Cấu hình trong `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "MySqlConnection": "Server=localhost;Port=3306;Database=ElectronyatShop;User=root;Password=;",
    "SqlServerConnection": "Server=(localdb)\\mssqllocaldb;Database=ElectronyatShop;Trusted_Connection=true;",
    "SqliteConnection": "Data Source=ElectronyatShop.db"
  }
}
```

### User Roles

Hệ thống có 2 vai trò chính:

- **Admin**: Quản lý toàn bộ hệ thống
- **Customer**: Khách hàng mua sắm

## 📊 Database Schema

Dự án sử dụng các bảng chính:

- **Products**: Thông tin sản phẩm
- **Orders**: Đơn hàng
- **OrderItems**: Chi tiết đơn hàng  
- **Carts**: Giỏ hàng
- **CartItems**: Sản phẩm trong giỏ hàng
- **AspNetUsers**: Người dùng (Identity)

## 🎯 Sử dụng

### Cho khách hàng

1. **Đăng ký/Đăng nhập**: Tạo tài khoản hoặc đăng nhập
2. **Duyệt sản phẩm**: Xem danh sách và chi tiết sản phẩm
3. **Thêm vào giỏ hàng**: Chọn sản phẩm và số lượng
4. **Thanh toán**: Xác nhận đơn hàng
5. **Theo dõi đơn hàng**: Xem trạng thái đơn hàng

### Cho quản trị viên

1. **Quản lý sản phẩm**: Thêm, sửa, xóa sản phẩm
2. **Quản lý đơn hàng**: Xem và cập nhật trạng thái đơn hàng
3. **Quản lý người dùng**: Phân quyền và quản lý tài khoản

## 🔨 Development

### Thêm Migration mới

```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

### Chạy tests

```bash
dotnet test
```

### Build production

```bash
dotnet publish -c Release
```

## 🐳 Docker Deployment

### Build Docker image

```bash
docker build -t electronyat-shop .
```

### Run container

```bash
docker run -p 8080:80 electronyat-shop
```

## 📝 Environment Variables

Cho production, cấu hình các biến môi trường:

```env
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__PostgresConnection=your_connection_string
```

## 🤝 Đóng góp

1. Fork dự án
2. Tạo feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Tạo Pull Request

## 📄 License

Dự án này được phân phối dưới giấy phép MIT. Xem file `LICENSE.txt` để biết thêm chi tiết.

## 📞 Liên hệ

- Email: khuongduy.works@gmail.com
- Project Link: https://github.com/duytdx/electronicShop.git
