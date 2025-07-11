# 🛒 'BulkyBook' E-Commerce Web Application (ASP.NET Core MVC)

This is a full-stack e-commerce web application developed using **ASP.NET Core MVC** as part of an internship program. The project is based on the [Complete ASP.NET Core and Entity Framework Development course on Udemy](https://www.udemy.com/course/complete-aspnet-core-21-course/?kw=mvc&src=sac&couponCode=24T3MT100725A).

## 🚀 Features

- Product listing and details pages
- Shopping cart functionality
- Order placement and checkout
- User registration and login
- Admin dashboard for managing products, categories, and orders
- Role-based access control (Admin / User)
- Responsive design using Bootstrap
- Database integration with Entity Framework Core
- ASP.NET Core Identity for user management

## 🛠️ Technologies Used

- ASP.NET Core MVC (.NET 8)
- Entity Framework Core (Code First)
- Razor Pages
- SQL Server
- ASP.NET Identity
- Bootstrap 5

## 📁 Project Structure

```
BulkyBook/
├── Bulky.DataAccess/        # Data access layer
│   ├── Data/               # Database context
│   ├── DbInitializer/      # Database seeding
│   ├── Migrations/         # EF Core migrations
│   └── Repository/         # Repository pattern implementation
├── Bulky.Models/           # Domain models and ViewModels
│   ├── ViewModels/         # View-specific models
│   └── ApplicationUser.cs  # User model extensions
├── Bulky.Utility/          # Utility classes
│   ├── EmailSender.cs      # Email functionality
│   └── SD.cs              # Static data and constants
├── BulkyWeb/              # Main web application
│   ├── Areas/             # MVC Areas
│   ├── Controllers/       # MVC Controllers
│   ├── ViewComponents/    # View components
│   ├── Views/            # Razor views
│   ├── wwwroot/          # Static files (CSS, JS, images)
│   ├── Program.cs        # Application entry point
│   └── appsettings.json  # Configuration settings
└── GitHub Actions/        # CI/CD workflows
```

## 📸 Screenshots

![alt text](home.png)

## 🔧 Installation & Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/sav0sav1c5/bulky-web-e-commerce-app.git
   cd bulky-web-e-commerce-app
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Update database connection string**
   - Open `appsettings.json` in the BulkyWeb project
   - Update the `ConnectionStrings:DefaultConnection` with your SQL Server details

4. **Run database migrations**
   ```bash
   dotnet ef database update
   ```
   Or in Visual Studio Package Manager Console:
   ```
   Update-Database
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```
   Or press F5 in Visual Studio

6. **Access the application**
   - Open your browser and navigate to `https://localhost:7099`

## 📚 Credits

This project was developed by following the [Complete ASP.NET Core and Entity Framework Development course](https://www.udemy.com/course/complete-aspnet-core-21-course/?kw=mvc&src=sac&couponCode=24T3MT100725A) on Udemy during an internship program.

This project is for educational purposes only.
