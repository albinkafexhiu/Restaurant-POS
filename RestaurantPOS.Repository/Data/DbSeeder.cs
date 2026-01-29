using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Domain.Entities;
using RestaurantPOS.Domain.Enums;
using RestaurantPOS.Repository.Data;

namespace RestaurantPOS.Web.Data
{
    public static class DbSeeder
    {
        private const string MainWaiterName = "Main Waiter";
        private const string MainWaiterPin = "1111";

        // NEW manager
        private const string ManagerName = "Manager";
        private const string ManagerPin = "9999";

        public static async Task SeedAsync(ApplicationDbContext db)
        {
            await db.Database.MigrateAsync();

            // Waiters (ensure at least 1 waiter + 1 manager)
            if (!await db.Waiters.AnyAsync())
            {
                db.Waiters.AddRange(
                    new Waiter
                    {
                        Id = Guid.NewGuid(),
                        FullName = MainWaiterName,
                        PinCode = MainWaiterPin,
                        IsActive = true,
                        IsManager = false
                    },
                    new Waiter
                    {
                        Id = Guid.NewGuid(),
                        FullName = ManagerName,
                        PinCode = ManagerPin,
                        IsActive = true,
                        IsManager = true
                    }
                );

                await db.SaveChangesAsync();
            }

            // Tables
            if (!await db.RestaurantTables.AnyAsync())
            {
                for (int i = 1; i <= 15; i++)
                {
                    db.RestaurantTables.Add(new RestaurantTable
                    {
                        Id = Guid.NewGuid(),
                        TableNumber = i,
                        Status = TableStatus.Free
                    });
                }

                await db.SaveChangesAsync();
            }

            // Categories (expanded)
            if (!await db.ProductCategories.AnyAsync())
            {
                db.ProductCategories.AddRange(
                    new ProductCategory { Id = Guid.NewGuid(), Name = "Drinks", DisplayOrder = 1 },
                    new ProductCategory { Id = Guid.NewGuid(), Name = "Food", DisplayOrder = 2 },
                    new ProductCategory { Id = Guid.NewGuid(), Name = "Desserts", DisplayOrder = 3 },
                    new ProductCategory { Id = Guid.NewGuid(), Name = "Coffee", DisplayOrder = 4 }
                );

                await db.SaveChangesAsync();
            }

            // Products (starter pack)
            if (!await db.Products.AnyAsync())
            {
                var categories = await db.ProductCategories.ToListAsync();

                Guid CatId(string name) =>
                    categories.First(c => c.Name == name).Id;

                db.Products.AddRange(
                    // Drinks
                    new Product { Id = Guid.NewGuid(), Name = "Coca-Cola", ProductCategoryId = CatId("Drinks"), Price = 80, IsAvailable = true },
                    new Product { Id = Guid.NewGuid(), Name = "Fanta", ProductCategoryId = CatId("Drinks"), Price = 80, IsAvailable = true },
                    new Product { Id = Guid.NewGuid(), Name = "Water", ProductCategoryId = CatId("Drinks"), Price = 50, IsAvailable = true },

                    // Coffee
                    new Product { Id = Guid.NewGuid(), Name = "Espresso", ProductCategoryId = CatId("Coffee"), Price = 70, IsAvailable = true },
                    new Product { Id = Guid.NewGuid(), Name = "Cappuccino", ProductCategoryId = CatId("Coffee"), Price = 90, IsAvailable = true },

                    // Food
                    new Product { Id = Guid.NewGuid(), Name = "Chicken Burger", ProductCategoryId = CatId("Food"), Price = 220, IsAvailable = true },
                    new Product { Id = Guid.NewGuid(), Name = "Cheeseburger", ProductCategoryId = CatId("Food"), Price = 240, IsAvailable = true },
                    new Product { Id = Guid.NewGuid(), Name = "Greek Salad", ProductCategoryId = CatId("Food"), Price = 180, IsAvailable = true },

                    // Desserts
                    new Product { Id = Guid.NewGuid(), Name = "Cheesecake", ProductCategoryId = CatId("Desserts"), Price = 150, IsAvailable = true },
                    new Product { Id = Guid.NewGuid(), Name = "Chocolate Cake", ProductCategoryId = CatId("Desserts"), Price = 160, IsAvailable = true }
                );

                await db.SaveChangesAsync();
            }
        }
    }
}
