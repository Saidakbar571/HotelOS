# 🏨 HotelOS — Hotel Management System

HotelOS is a full-stack hotel management system built with ASP.NET Core and vanilla JavaScript. It supports multiple roles (receptionist, guest, housekeeper) and handles the full booking lifecycle.

## 🚀 Tech Stack

- **Backend:** ASP.NET Core, C#
- **Frontend:** HTML, CSS, Vanilla JS (SPA)
- **Database:** In-memory (AppDatabase)
- **Background Jobs:** IHostedService (ExpiredBookingWorker)
- **API Docs:** Scalar UI (`/scalar/v1`)

## 👥 Demo Accounts

| Role | Account ID | Password |
|------|-----------|----------|
| 💼 Receptionist | REC001 | admin123 |
| 👤 Guest | GST001 | guest123 |
| 🧹 Housekeeper | HK001 | hk123 |

## ✨ Features

- **Dashboard** — live room stats, revenue, pending tasks
- **Rooms** — add, view, update room status
- **Bookings** — 2-step hold + payment flow (10-min timer)
- **Check-in / Check-out** — with automatic invoice generation
- **Cleaning** — task assignment and completion tracking
- **Services** — add room/kitchen/amenity services to bookings
- **Guests** — register and manage guest accounts
- **Branches** — multi-branch support

## 🔄 Booking Flow

Hold (10 min) → Payment → Confirmed

↓ (timeout)

Abandoned  ← Worker (every 60s)

## 🏃 Run Locally

```bash
dotnet run
```

Open: `http://localhost:5000`

## 📁 Project Structure
HotelOS/

├── API/Controllers/        # REST endpoints

├── Application/

│   ├── Services/           # Business logic

│   └── Interfaces/         # Service contracts

├── Domain/

│   ├── Entities/           # Core models

│   └── Enums/              # Status enums

├── Infrastructure/

│   ├── Persistence/        # In-memory database

│   └── BackgroundJobs/     # ExpiredBookingWorker

└── wwwroot/                # SPA frontend

├── index.html

├── css/style.css

└── js/app.js

## 👨‍💻 Author

**Saidakbar** — PDP University, BTEC HND Digital Technologies
