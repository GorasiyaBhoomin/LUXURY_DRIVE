# Luxury Drive - Project Q&A

This document contains expected technical and non-technical questions along with detailed answers for the **Luxury Drive** project. This is especially useful for viva, presentations, and interviews.

---

## 🏎️ Non-Technical Questions

### 1. What is the business purpose of the "Luxury Drive" project?
**Answer:** 
Luxury Drive is a modern online vehicle rental platform. Its main purpose is to provide a seamless, premium experience for customers looking to browse and rent luxury and standard vehicles. It also offers a robust backend administration panel for adding vehicles, managing customer bookings, tracking vehicle specifications, and communicating with customers.

### 2. Who is the target audience for this platform?
**Answer:** 
The platform serves two primary audiences:
- **Customers/End Users:** Individuals looking to rent vehicles easily with transparent details (features, seat capacity, fuel type, transmission, etc.).
- **Administrators/Fleet Managers:** The business owners or managers who need an organized dashboard to monitor bookings, manage users, and maintain the vehicle fleet catalog.

### 3. What are the key features of the system?
**Answer:**
- A dynamic, data-driven catalog for vehicles highlighting key specifications (Seats, Color, Fuel, Transmission, Year).
- A customer portal with a personalized booking management history and details.
- Secure user authentication and cookie-based sessions.
- An intuitive Admin panel for CRUD (Create, Read, Update, Delete) operations on vehicles, customers, and bookings.
- Integrated invoice/PDF generation for bookings.
- Email triggers directly from the administrator's dashboard (via mailto/SMTP integrations).

### 4. What were some of the challenges faced during development and how were they solved?
**Answer:** 
One major challenge was maintaining database consistency when adding new vehicle features (Seats, Transmission, Fuel, Year, Color, and Key Features). This was solved by keeping the C# Entity models strictly synchronized with the database using **Entity Framework Core Migrations**. Another challenge was ensuring that dynamic UI components correctly received and rendered the right model data, which was handled through proper ASP.NET MVC data binding and ViewModels.

---

## 💻 Technical Questions

### 1. What architecture does the Luxury Drive system follow?
**Answer:** 
The project follows the **MVC (Model-View-Controller)** design pattern using the **ASP.NET Core** framework. 
- **Models:** Represent data structures (e.g., `Vehicle`, `Customer`, `User`, `Booking`) and database entities.
- **Views:** Handle the UI layout (`.cshtml` Razor pages) focusing heavily on dynamic C# code execution integrated directly with HTML.
- **Controllers:** Handle logic, process user requests (e.g., `HomeController.cs`, `AdminController.cs`, `AuthenticationController.cs`), connect to the database, and return data to the relevant view.

### 2. How did you handle Database interactions?
**Answer:** 
I used **Entity Framework Core (EF Core)** using a **Code-First** approach. By creating an `AppDbContext` class, EF Core translates my C# models into underlying SQL Server tables. I used migrations (`Add-Migration` and `Update-Database`) to securely update database schemas when models changed.

### 3. How is user authentication managed in the project?
**Answer:** 
The project manages user sessions via **Cookie-based Authentication** using the built-in Microsoft Authentication API (`Microsoft.AspNetCore.Authentication.Cookies`). The `Program.cs` is configured with `builder.Services.AddAuthentication().AddCookie()`, keeping tracking of user sessions, restricting access to unauthorized pages via the `[Authorize]` attribute, and redirecting unauthenticated users to the `/Authentication/Login` route.

### 4. How are PDF exports or Document Generations handled?
**Answer:** 
The system utilizes **QuestPDF**, an open-source .NET library used for creating highly structured and complex PDF documents. This is configured in `Program.cs` utilizing the `QuestPDF.Infrastructure.LicenseType.Community` license. It provides an API to fluidly draw layouts for booking receipts, fleet reports, or customer details directly from the ASP.NET controllers.

### 5. How are views rendered dynamically based on database properties?
**Answer:** 
Using **Razor Syntax (`@`)**, the application renders loops and conditional statements directly in the view. For instance, in `AdminVehicles.cshtml` or `Car_view.cshtml`, an `@model List<Vehicle>` or `IEnumerable` directive is declared at the top of the file, allowing me to iterate using `@foreach(var item in Model)` to dynamically draw HTML elements for each database row.

### 6. Where is the connection string stored and how is it loaded securely?
**Answer:** 
The Database connection string is stored securely inside the `appsettings.json` file under `ConnectionStrings:DefaultConnection`. It is loaded at runtime via dependency injection inside `Program.cs` using `builder.Configuration.GetConnectionString("DefaultConnection")`.

---

## 🔌 APIs, Services, and Packages Included

Below are the primary packages, services, and APIs powering the Luxury Drive platform:

1. **Entity Framework Core SQL Server (`Microsoft.EntityFrameworkCore.SqlServer`)**
   - **Role:** Providing native interaction with MS SQL Server databases using EF Core context and LINQ (Language Integrated Query) without manually writing raw SQL queries.

2. **ASP.NET Core Cookies Authentication**
   - **Role:** Handles encrypting user claims into secure browser cookies to persist login state across HTTP requests. Managed via `UseAuthentication` and `UseAuthorization` middlewares.

3. **QuestPDF**
   - **Role:** Exposes an API used to generate dynamic PDF documents visually using a declarative C# layout system.

4. **Dynamic Data Binding / Routing APIs**
   - **Role:** Found in ASP.NET MVC's core `MapControllerRoute`, routing API binds requests like `/Admin/AdminVehicles` securely to the specific `IActionResult` methods on the backend.

5. **Mailto / Front-end Action Handlers**
   - **Role:** The system leverages HTML5 integrated URI schemes (`mailto:`) to seamlessly hook into natively installed email clients (like Outlook or Gmail) for instant admin-to-customer communications without a heavy SMTP backend footprint.
