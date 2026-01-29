# Restaurant POS System

Simple restaurant point-of-sale system built for the Integrated Systems course.

The app allows waiters to log in with a PIN, select tables, open orders and add menu items. Orders can be closed with payment methods. There is an admin panel for managing tables, products, categories, waiters and expenses. The system also integrates an external meals API to suggest dishes that can be imported into the menu.

## Live Demo

Try the app here:  
https://restaurant-pos-nc9z.onrender.com/

## Test Credentials

Waiter PIN: `1111`  
Manager PIN: `9999`

## Tech Stack

- ASP.NET Core MVC (.NET 10)
- Entity Framework Core with SQLite
- Onion architecture:
  - RestaurantPOS.Domain
  - RestaurantPOS.Repository
  - RestaurantPOS.Service
  - RestaurantPOS.Web

## Features

- Waiter PIN authentication
- Table-based ordering
- Product and category management
- Manager/admin panel
- Expenses tracking
- External meals API integration
- SQLite database
- Docker deployment

## Running Locally

```bash
dotnet restore
dotnet run --project RestaurantPOS.Web
