# Restaurant POS System

Simple restaurant point of sale system for the Integrated Systems course.

The app lets waiters log in with a PIN, select a table, open an order and add menu items. Orders can be closed with a payment method. There is basic admin for tables, products, categories, waiters and expenses. The system will also use an external meals API to suggest dishes that can be added to the menu.

## Tech stack

- ASP.NET Core MVC (.NET 10)
- Entity Framework Core with SQLite
- Onion style architecture
    - RestaurantPOS.Domain
    - RestaurantPOS.Repository
    - RestaurantPOS.Service
    - RestaurantPOS.Web

## How to run

1. Clone the repo.
2. Open the solution in Rider.
3. Restore NuGet packages.
4. From the solution folder run:

   ```bash
   dotnet ef database update \
     --project RestaurantPOS.Repository/RestaurantPOS.Repository.csproj \
     --startup-project RestaurantPOS.Web/RestaurantPOS.Web.csproj
