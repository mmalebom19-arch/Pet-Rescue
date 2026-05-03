# 🐾 RescuePet – Pet Adoption & Rescue Platform

A full-stack web application built with **ASP.NET MVC** and **SQL Server** that connects rescue pets with loving adopters. Users can browse available pets, submit adoption requests, post pets for rescue, and donate to a fundraising goal.

---

## ✨ Features

- **Browse & Filter Pets** — search by animal type, breed, and location using dynamic dropdown filters
- **Pet Adoption** — submit adoption requests that update pet status in the database in real time
- **Post a Pet** — list a rescue pet with photo upload, breed, age, weight, and backstory
- **Donations Tracker** — donate to a R50,000 fundraising goal with a live progress bar and donor leaderboard
- **Home Dashboard** — displays adoption count and recent adoption history

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET MVC 5 (.NET Framework 4.7.2) |
| Language | C# |
| Frontend | Razor Views, HTML, CSS, Bootstrap 5, jQuery |
| Database | Microsoft SQL Server (SQL Express) |
| ORM / Data Access | ADO.NET (SqlConnection, SqlCommand, SqlDataReader) |
| Image Handling | Server-side file upload & storage |

---

## 📁 Project Structure

```
RescuePet/
│
├── Controllers/
│   ├── HomeController.cs        # Dashboard — adoption count & recent adoptions
│   ├── PetsController.cs        # Pet listing, filtering, adoption, posting
│   └── DonationsController.cs   # Donation submission & progress tracker
│
├── Views/
│   ├── Home/                    # Index (dashboard), About, Contact
│   ├── Pets/                    # Pets listing, Adopt, Post a pet
│   ├── Donations/               # Donation form & tracker
│   └── Shared/                  # Shared layout & error page
│
├── Content/                     # CSS & Bootstrap styles
├── Scripts/                     # jQuery & Bootstrap JS
├── Images/                      # Uploaded pet images
└── RescuePet.sql                # Database schema & seed data
```

---

## 🗄️ Database Schema

The SQL Server database includes the following tables:

- **Pets** — pet details (name, age, weight, gender, story, status, image path)
- **Pet_Types** — animal type categories (e.g. Dog, Cat)
- **Pet_Breeds** — breeds linked to types
- **Locations** — rescue locations
- **Users** — adopters and pet posters
- **Adoptions** — adoption records linking users and pets
- **Donations** — donation records with amounts and dates

---

## ⚙️ Getting Started

### Prerequisites
- Visual Studio 2022
- SQL Server Express
- .NET Framework 4.7.2

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/YOUR_USERNAME/rescue-pet.git
   ```

2. **Set up the database**
   - Open SQL Server Management Studio (SSMS)
   - Run `RescuePet.sql` to create and seed the database

3. **Update the connection string**
   - In each Controller file, update the `ConnectionString` to match your SQL Server instance:
   ```csharp
   public static string ConnectionString = 
     "Data Source=YOUR_SERVER_NAME\\SQLEXPRESS;Initial Catalog=RescuePet;Integrated Security=True";
   ```

4. **Run the project**
   - Open `u24608174_Assignment_02.sln` in Visual Studio
   - Press **F5** to build and run

---

## 🧠 Key Technical Highlights

- **Dynamic SQL filtering** — pet search uses parameterised queries with conditional `WHERE` clauses to prevent SQL injection
- **Image upload pipeline** — uploaded pet photos are saved server-side with a GUID filename and served as static assets
- **Cascading dropdowns** — breed list updates dynamically via AJAX (`JsonResult`) based on selected animal type
- **Donation progress tracker** — aggregates donation totals in real time, calculates percentage to goal, and displays remaining amount

---

## 📚 Module Context
Built as part of **INF 272 ** at the University of Pretoria (2025).

---

## 👩🏽‍💻 Author
**Maria Malebo Maleka** — [LinkedIn](https://linkedin.com/in/maria-malebo-maleka-5a405635b)

> *Academic project — connection strings and local paths will need updating to run in a new environment.*
