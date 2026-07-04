using HyliCoffeeWeb.Models;
using HyliCoffeeWeb.Services;
using Microsoft.EntityFrameworkCore;

namespace HyliCoffeeWeb.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext context, IPasswordHasher hasher)
        {
            // ===== 1. Tài khoản Admin =====
            if (!await context.Users.AnyAsync(u => u.Username == "admin"))
            {
                context.Users.Add(new User
                {
                    Username = "admin",
                    Password = hasher.Hash("Admin@123"),
                    FullName = "Quản trị viên",
                    Email    = "admin@hylicoffee.vn",
                    Phone    = "0900000000",
                    Role     = UserRole.Admin
                });
            }

            // ===== 2. Tài khoản khách demo =====
            if (!await context.Users.AnyAsync(u => u.Username == "customer1"))
            {
                context.Users.Add(new User
                {
                    Username = "customer1",
                    Password = hasher.Hash("Customer@123"),
                    FullName = "Nguyễn Văn A",
                    Email    = "customer1@gmail.com",
                    Phone    = "0911111111",
                    Role     = UserRole.Customer
                });
            }

            // ===== 3. Sản phẩm mẫu =====
            // FIX LỖI 6: cfsua.jpg và tradao.jpg KHÔNG tồn tại trong wwwroot/img/
            // → thay bằng ảnh thật đang có: cafe.jpg và tim.jpg
            if (!await context.Products.AnyAsync())
            {
                context.Products.AddRange(
                    new Product
                    {
                        Name = "Phê Đen Phin (Traditional Soul)", Price = 22000,
                        Type = "Cà phê", Image = "cfden.jpg", Featured = true,
                        Description = "Những giọt cà phê nguyên chất được chắt lọc chậm rãi qua từng nhịp phin nhôm truyền thống."
                    },
                    new Product
                    {
                        // cfsua.jpg không có → dùng cafe.jpg
                        Name = "Cà Phê Sữa Đá", Price = 25000,
                        Type = "Cà phê", Image = "cafe.jpg", Featured = true,
                        Description = "Vị đắng đậm đà hòa quyện cùng sữa đặc béo ngậy."
                    },
                    new Product
                    {
                        // tradao.jpg không có → dùng tim.jpg (màu đỏ hồng giống trà đào)
                        Name = "Trà Đào Cam Sả", Price = 35000,
                        Type = "Trà", Image = "tim.jpg", Featured = true,
                        Description = "Thanh mát, giải nhiệt, vị đào chua ngọt hài hòa."
                    },
                    new Product
                    {
                        Name = "Matcha Popcorn", Price = 39000,
                        Type = "Trà", Image = "matcha.jpg", Featured = true,
                        Description = "Sự kết hợp độc đáo giữa matcha Nhật Bản và bắp rang bơ."
                    },
                    new Product
                    {
                        Name = "Vú Sữa Dầm", Price = 32000,
                        Type = "Đặc biệt", Image = "do.jpg", Featured = false,
                        Description = "Món thức uống lạ miệng, sáng tạo từ trái vú sữa tươi."
                    },
                    new Product
                    {
                        Name = "Mèo Phê Pha", Price = 200000,
                        Type = "Tranh meme", Image = "mew1.png", Featured = false,
                        Description = "Bạn đang bị sếp dí deadline? Nhìn vào biểu cảm em mèo này, mọi lo toan sẽ về số 0 ngay lập tức."
                    },
                    new Product
                    {
                        Name = "Mèo Logic", Price = 200000,
                        Type = "Tranh meme", Image = "mew2.png", Featured = false,
                        Description = "Mệt mỏi vì cuộc sống quá khắc nghiệt? Em mèo này sẽ chữa lành mọi năng lượng tiêu cực."
                    },
                    new Product
                    {
                        Name = "Mèo Tự Tin", Price = 200000,
                        Type = "Tranh meme", Image = "mew3.png", Featured = false,
                        Description = "Nhìn tui xinh không? Xinh đúng không? Đời có thể yêu thương mình bằng nắm đấm, nhưng mình vẫn phải xinh!"
                    }
                );
            }

            // ===== 4. Bài viết mẫu =====
            // FIX LỖI 6: blog-1.jpg và blog-2.jpg KHÔNG tồn tại
            // → thay bằng article.jpg và cafe.jpg
            if (!await context.Articles.AnyAsync())
            {
                context.Articles.AddRange(
                    new Article
                    {
                        Title = "Lịch sử cây trà trên thế giới",
                        Category = "Kiến thức",
                        Image = "article.jpg",   // blog-1.jpg → article.jpg
                        Content = "Trà là một trong những thức uống lâu đời nhất, có nguồn gốc từ Trung Quốc cách đây hàng nghìn năm...",
                        CreatedDate = DateTime.Now.AddDays(-10)
                    },
                    new Article
                    {
                        Title = "Vì sao cà phê phin truyền thống vẫn được yêu thích",
                        Category = "Cà phê",
                        Image = "cafe.jpg",      // blog-2.jpg → cafe.jpg
                        Content = "Phin cà phê nhôm truyền thống mang lại hương vị đậm đà mà ít phương pháp pha chế nào sánh được...",
                        CreatedDate = DateTime.Now.AddDays(-5)
                    }
                );
            }

            await context.SaveChangesAsync();
        }
    }
}
