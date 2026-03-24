# 🚘 LUXURY DRIVE

> A premium luxury car rental web application built with **ASP.NET Core MVC (.NET 10)**

![Hero](https://images.unsplash.com/photo-1503376780353-7e6692767b70?w=1200&q=80&fit=crop)

---

## ✨ Features

### 👤 Customer Portal
- Browse & view luxury vehicles
- Book a car with date & duration selection
- Manage personal profile & change password
- Contact form for inquiries

### 🛡️ Admin Panel
| Module | Description |
|---|---|
| Dashboard | Overview of bookings, revenue & stats |
| Bookings | View, filter & manage all customer bookings |
| Vehicles | Add, edit & remove fleet vehicles |
| Customers | View registered customer list |
| Profile | Admin profile & password management |

---

## 📸 Screenshots

### Home Page
![Home](https://images.unsplash.com/photo-1492144534655-ae79c964c9d7?w=900&q=80&fit=crop)

### Vehicle Listing
![Vehicles](https://images.unsplash.com/photo-1555215695-3004980ad54e?w=900&q=80&fit=crop)

### Admin Dashboard
![Dashboard](https://images.unsplash.com/photo-1551288049-bebda4e38f71?w=900&q=80&fit=crop)

### Bookings Management
![Bookings](https://images.unsplash.com/photo-1606664515524-ed2f786a0bd6?w=900&q=80&fit=crop)

---

## 🛠️ Tech Stack

- **Framework:** ASP.NET Core MVC — .NET 10
- **Language:** C#
- **Frontend:** Razor Views, HTML5, CSS3, JavaScript
- **Fonts:** Google Fonts (DM Sans)
- **Architecture:** MVC Pattern

---

## 📁 Project Structure

```
LUXURY_DRIVE/
├── Controllers/
│   ├── Admin.cs               # Admin panel actions
│   ├── AuthenticationController.cs
│   └── HomeController.cs      # Customer-facing actions
├── Models/
│   ├── CarRentViewModel.cs
│   ├── VehicleViewModel.cs
│   ├── LoginViewModel.cs
│   ├── RegisterViewModel.cs
│   └── ...
├── Views/
│   ├── Admin/                 # Dashboard, Bookings, Vehicles, Customers, Profile
│   ├── Authentication/        # Login, Register
│   ├── Home/                  # Index, Car_view, Car_rent, Profile
│   └── Shared/
└── wwwroot/
    └── css/                   # Per-page stylesheets
```

---

## 🚀 Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Run Locally

```bash
git clone https://github.com/<your-username>/LUXURY_DRIVE.git
cd LUXURY_DRIVE
dotnet run
```

App starts at `https://localhost:5001` — default route opens the **Login** page.

---

## 🔐 Default Routes

| URL | Page |
|---|---|
| `/Authentication/Login` | Login |
| `/Authentication/Register` | Register |
| `/Home/Index` | Home / Landing |
| `/Home/Car_view` | Browse Vehicles |
| `/Home/Car_rent` | Book a Vehicle |
| `/Admin/Dashboard` | Admin Dashboard |
| `/Admin/Adminbookings` | Manage Bookings |
| `/Admin/AdminVehicles` | Manage Fleet |
| `/Admin/AdminCustomers` | Manage Customers |

---

## 🚗 Fleet Preview

| | | |
|---|---|---|
| ![Car1](https://images.unsplash.com/photo-1544636331-e26879cd4d9b?w=300&q=70&fit=crop) | ![Car2](https://images.unsplash.com/photo-1580274455191-1c62238fa333?w=300&q=70&fit=crop) | ![Car3](https://images.unsplash.com/photo-1618843479313-40f8afb4b4d8?w=300&q=70&fit=crop) |
| Mercedes-Benz S-Class | Lamborghini Huracán | BMW 7 Series |

---

## 📄 License

This project is for academic purposes — 6th Semester B.Tech (.NET).
