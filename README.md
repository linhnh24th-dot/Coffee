# HyliCoffeeWeb — ASP.NET Core MVC (.NET 8)

Chuyển đổi từ template tĩnh **tea-shop-website-template** (HTML/CSS/JS + AdminLTE) sang
ASP.NET Core MVC + EF Core + SQL Server, giữ nguyên 100% giao diện, CSS, animation gốc.

## 1. Yêu cầu môi trường

- .NET 8 SDK
- SQL Server (LocalDB / Express / Full) — database tên `sellitemQuanLy`
- Visual Studio 2022 (17.8+) hoặc VS Code + C# Dev Kit

## 2. Cấu hình Connection String

Mở `appsettings.json`, sửa lại cho đúng máy của bạn:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=sellitemQuanLy;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

Nếu dùng SQL Authentication thay vì Windows Authentication:

```
Server=.;Database=sellitemQuanLy;User Id=sa;Password=YourPassword;TrustServerCertificate=True
```

## 3. Khôi phục package & tạo Migration

Mở terminal tại thư mục gốc `HyliCoffeeWeb/` (chứa file `.csproj`):

```bash
dotnet restore

# Cài EF Core CLI tool (nếu chưa có)
dotnet tool install --global dotnet-ef

# Tạo migration đầu tiên dựa trên các Model đã có
dotnet ef migrations add InitialCreate

# Áp dụng migration để tạo database sellitemQuanLy + toàn bộ bảng
dotnet ef database update
```

> Lưu ý: Bạn **không bắt buộc** phải chạy `dotnet ef database update` thủ công —
> `Program.cs` đã cấu hình `db.Database.Migrate()` để tự động migrate khi chạy app
> lần đầu tiên (`dotnet run`). Tuy nhiên migration files (`Migrations/*.cs`) vẫn cần
> được tạo bằng lệnh `dotnet ef migrations add` ở trên trước, vì migration files
> chưa được sinh sẵn trong gói code này (phụ thuộc đúng phiên bản EF Core Tools cài
> trên máy bạn).

## 4. Chạy ứng dụng

```bash
dotnet run
```

Lần chạy đầu tiên, ứng dụng sẽ tự động:
1. Migrate database `sellitemQuanLy` (tạo toàn bộ bảng: Products, Articles, Users, Orders, OrderDetails, Staffs, Feedbacks).
2. Seed dữ liệu mẫu (`Data/DbInitializer.cs`):
   - Tài khoản Admin: `admin` / `Admin@123`
   - Tài khoản khách demo: `customer1` / `Customer@123`
   - 8 sản phẩm mẫu (cà phê, trà, tranh meme...) lấy đúng nội dung từ source HTML gốc
   - 2 bài viết tin tức mẫu

Truy cập:
- Trang khách: `https://localhost:xxxx/`
- Trang quản trị (yêu cầu đăng nhập role Admin): `https://localhost:xxxx/Admin`

## 5. Cấu trúc dự án

```
HyliCoffeeWeb/
├── Controllers/         # Home, Product, Cart, Account, Order (khách hàng)
├── Areas/Admin/
│   ├── Controllers/     # Dashboard, Product, Article, Staff, UserMgmt, Order, Feedback
│   └── Views/           # Giao diện AdminLTE giữ nguyên
├── Models/               # Product, Article, User, Order, OrderDetail, Staff, Feedback
├── ViewModels/           # LoginViewModel, RegisterViewModel, CheckoutViewModel, DashboardViewModel
├── Data/
│   ├── ApplicationDbContext.cs
│   └── DbInitializer.cs # Seed dữ liệu mẫu
├── Services/
│   ├── PasswordHasher.cs  # Hash mật khẩu PBKDF2
│   └── CartService.cs     # Giỏ hàng lưu Session (thay localStorage)
├── Views/                # Giao diện khách hàng giữ nguyên CSS/JS gốc
├── wwwroot/
│   ├── css, js, lib, img  # Giữ nguyên 100% từ template gốc
│   └── admin/css, admin/js # AdminLTE dist (giữ nguyên 100%)
├── appsettings.json
└── Program.cs
```

## 6. Những thay đổi cốt lõi so với bản HTML/JS gốc

| Bản gốc (JS thuần) | Bản chuyển đổi (.NET 8 MVC) |
|---|---|
| `localStorage.getItem('products')` để hiển thị sản phẩm | `ProductController` đọc từ SQL Server qua EF Core |
| `sessionStorage.getItem('admin_login')` để bảo vệ trang admin | `[Authorize(Policy = "AdminOnly")]` + Cookie Authentication + Role |
| Không có giỏ hàng/đăng nhập/đơn hàng thật | `CartService` (Session) + `AccountController` (Cookie Auth) + `Order`/`OrderDetail` (EF Core) |
| CRUD sản phẩm/nhân viên bằng JS thao tác mảng trong browser | CRUD thật qua `Areas/Admin/Controllers` + EF Core + SQL Server |

## 7. Đăng nhập thử nghiệm

| Vai trò | Username | Password |
|---|---|---|
| Admin | `admin` | `Admin@123` |
| Customer | `customer1` | `Customer@123` |

**Lưu ý bảo mật:** Hãy đổi mật khẩu Admin mặc định ngay sau khi triển khai thực tế.

## 8. Deploy lên Somee.com (hoặc IIS bất kỳ) — fix lỗi "bấm vào link bị Not Found"

Lỗi bạn gặp (trang chủ load được nhưng bấm About/menu khác thì 404) gần như luôn do
**một trong 3 nguyên nhân sau** khi host trên IIS/Somee:

### Nguyên nhân 1: Upload nhầm source code thay vì bản publish
Somee (và IIS nói chung) **không chạy trực tiếp source `.cs`/`.cshtml`**. Bạn phải build
ra bản publish rồi upload đúng các file đó:

```bash
dotnet publish -c Release -o publish
```

Sau đó upload **toàn bộ nội dung thư mục `publish/`** (gồm `HyliCoffeeWeb.dll`,
`web.config`, `wwwroot/`, các file `.dll` phụ thuộc...) lên thư mục gốc site trên Somee —
không upload thư mục `Controllers/`, `Views/` (`.cshtml`) dạng source thô.

### Nguyên nhân 2: Thiếu `web.config` (đã bổ sung trong gói này)
Tôi đã thêm file `web.config` ở thư mục gốc dự án. File này bắt buộc để IIS biết
forward toàn bộ request (kể cả URL không có phần mở rộng như `/Home/About`) sang
ASP.NET Core qua module `AspNetCoreModuleV2`. Thiếu file này, IIS chỉ phục vụ được
file tĩnh có thật trên đĩa (đúng lý do trang chủ "có vẻ chạy" nhưng các route khác 404).

Khi `dotnet publish`, file `web.config` này sẽ tự được copy vào thư mục `publish/`.

Nếu sau khi deploy vẫn lỗi, vào Somee bật xem log lỗi chi tiết: tạo thư mục `logs/`
cùng cấp `web.config` (đã có sẵn trong gói) rồi mở file `logs/stdout_*.log` để xem lỗi thật.

### Nguyên nhân 3: Somee chưa bật đúng phiên bản .NET 8
Trong control panel Somee, kiểm tra mục cấu hình ASP.NET Core / .NET version của site,
chọn đúng **.NET 8 (LTS)**. Nếu Somee chỉ hỗ trợ .NET Framework cho gói bạn đang dùng,
bạn cần đổi sang gói/server có hỗ trợ ASP.NET Core, vì đây không phải ASP.NET Framework
(Web Forms/MVC5) mà là ASP.NET Core — hai nền tảng khác nhau, IIS cần module khác nhau.

### Đã sửa thêm trong `Program.cs`
- Thêm `UseForwardedHeaders` để app nhận đúng scheme/host khi chạy sau IIS reverse proxy (Somee).
- Bỏ ép `UseHttpsRedirection` ở môi trường Production (chỉ bật khi chạy Development cục bộ) — vì
  SSL đã được IIS xử lý ở tầng ngoài, ép thêm lần nữa dễ gây lặp redirect hoặc sai URL khiến
  các trang con như About bị "Not Found".
- Thêm `UseStatusCodePagesWithReExecute` để khi có lỗi 404/500 sẽ hiện trang lỗi rõ ràng
  (`/Home/Error`) thay vì màn hình trắng khó đoán nguyên nhân.

Sau khi áp dụng cả 3 mục trên (publish đúng cách + có `web.config` + đúng .NET version),
link About và toàn bộ các trang khác sẽ hoạt động bình thường.
