# Car Dealership Project

A Windows Forms application for a car dealership with user login/register and a rotating carousel display.

## 🚗 Features

- User Login/Registration with SQL Server
- Rotating carousel of cars with transparent PNG images
- Previous/Next navigation buttons
- Select car to open purchase dialog
- Discord-inspired dark theme

## 📋 Prerequisites

- Visual Studio 2022
- SQL Server (LocalDB, SQL Express, or full SQL Server)
- .NET Framework (or .NET Core)

## 🗄️ Database Setup

Run this SQL script in SQL Server Management Studio (SSMS) to create the database and tables:

```sql
-- =============================================
-- COMPLETE CAR DEALERSHIP DATABASE SETUP
-- Creates database, tables, and inserts sample data
-- WITH LOCAL CarImages FOLDER PATHS
-- =============================================

-- Create database (if it doesn't exist)
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'CarDealershipDB')
BEGIN
    CREATE DATABASE CarDealershipDB;
    PRINT 'Database created successfully';
END
ELSE
BEGIN
    PRINT 'Database already exists';
END
GO

-- Use the database
USE CarDealershipDB;
GO

-- =============================================
-- CREATE USERS TABLE (for login/register)
-- =============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
BEGIN
    CREATE TABLE Users (
        UserID INT PRIMARY KEY IDENTITY(1,1),
        Username NVARCHAR(50) NOT NULL UNIQUE,
        Password NVARCHAR(50) NOT NULL
    );
    PRINT 'Users table created';
    
    -- Insert test users
    INSERT INTO Users (Username, Password) VALUES ('admin', 'admin123');
    INSERT INTO Users (Username, Password) VALUES ('john', 'password123');
    PRINT 'Test users added';
END
ELSE
BEGIN
    PRINT 'Users table already exists';
END
GO

-- =============================================
-- CREATE CARS TABLE
-- =============================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Cars' AND xtype='U')
BEGIN
    CREATE TABLE Cars (
        CarID INT PRIMARY KEY IDENTITY(1,1),
        ModelName NVARCHAR(100) NOT NULL,
        Price DECIMAL(10,2) NOT NULL,
        ImageUrl NVARCHAR(500) NOT NULL,
        Year INT,
        Description NVARCHAR(MAX)
    );
    PRINT 'Cars table created';
END
ELSE
BEGIN
    PRINT 'Cars table already exists - clearing old data';
    DELETE FROM Cars;
END
GO

-- =============================================
-- INSERT CARS WITH LOCAL CarImages FOLDER PATHS
-- Using transparent PNG files from CarImages folder
-- =============================================
INSERT INTO Cars (ModelName, Price, ImageUrl, Year, Description)
VALUES 
('Toyota Camry', 2657000.00, 'CarImages\\toyota-camry-removebg-preview.png', 2023, 'Reliable family sedan with transparent background'),
('Honda Civic', 1583000.00, 'CarImages\\honda-civic-removebg-preview.png', 2023, 'Sporty compact with transparent background'),
('Ford Mustang', 3506000.00, 'CarImages\\ford-mustang-removebg-preview.png', 2022, 'Iconic American muscle with transparent background'),
('Tesla Model 3', 1838000.00, 'CarImages\\tesla-model-3-removebg-preview.png', 2023, 'Electric performance with transparent background'),
('BMW X5', 5990000.00, 'CarImages\\bmw-x5-removebg-preview.png', 2022, 'Luxury SUV with transparent background');
GO

PRINT 'Sample cars inserted with local image paths';
GO

-- =============================================
-- VERIFY ALL DATA
-- =============================================
PRINT '=== USERS TABLE ===';
SELECT UserID, Username, Password FROM Users;
GO

PRINT '=== CARS TABLE ===';
SELECT 
    CarID,
    ModelName,
    '₱' + FORMAT(Price, 'N0') AS Price_PHP,
    ImageUrl AS Local_Image_Path,
    Year,
    Description
FROM Cars;
GO
```

## 🔧 Configuration

### Connection String

Update the connection string in `LoginForm.cs` to match your SQL Server instance:

```csharp
_connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=CarDealershipDB;Integrated Security=True;";
```

Common options:
- `.\SQLEXPRESS` - SQL Server Express
- `(localdb)\MSSQLLocalDB` - LocalDB (comes with Visual Studio)
- `localhost` - Full SQL Server
- `DESKTOP-ABC123` - Your computer name

### 📁 Folder Structure

Make sure your project has this folder structure for the images:

```
YourProject\bin\Debug\
├── CarImages\           ← Create this folder
│   ├── toyota-camry-removebg-preview.png
│   ├── honda-civic-removebg-preview.png
│   ├── ford-mustang-removebg-preview.png
│   ├── tesla-model-3-removebg-preview.png
│   └── bmw-x5-removebg-preview.png
└── CarDealership.exe
```

## 🚀 How to Run

1. Clone this repository
2. Run the SQL script above in SSMS to create the database
3. Create the `CarImages` folder in `bin\Debug\` and add the PNG files
4. Update the connection string in `LoginForm.cs`
5. Build and run the project
6. Login with:
   - Username: `admin`
   - Password: `admin123`

## 🎨 Color Scheme

The app uses a Discord-inspired red theme:
- Primary buttons: `#F04747` (Red)
- Hover state: `#D93737` (Darker red)
- Background: Dark grays (`#2C2F33`, `#202225`)

## 📸 Screenshots

(Add screenshots here)

## 📝 License

This project is licensed under the MIT License.
