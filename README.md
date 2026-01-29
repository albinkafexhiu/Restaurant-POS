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

   ```bash
   https://restaurant-pos-nc9z.onrender.com/
