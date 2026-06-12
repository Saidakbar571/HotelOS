// Program.cs
// ASP.NET Core 10 — Minimal hosting model
//
// .NET 10 o'zgarishlari:
//   - Swashbuckle o'rniga built-in OpenAPI + Scalar UI
//   - app.MapOpenApi()  → /openapi/v1.json
//   - Scalar UI         → /scalar/v1  (Swagger o'rniga)

using HotelOS.Application.Interfaces;
using HotelOS.Application.Services;
using HotelOS.Infrastructure.BackgroundJobs;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ── Controllers ───────────────────────────────────────────
builder.Services.AddControllers();

// ── OpenAPI (.NET 10 built-in) ────────────────────────────
// Swashbuckle .NET 10 da ishlamaydi
// MapOpenApi() → /openapi/v1.json
// AddScalarApiReference() → /scalar/v1
builder.Services.AddOpenApi();

// ── In-Memory Cache — HoldService uchun ─────────────────
builder.Services.AddMemoryCache();

// ── CORS ─────────────────────────────────────────────────
builder.Services.AddCors(o =>
    o.AddDefaultPolicy(p => p
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()));

// ── Dependency Injection ──────────────────────────────────
//
// Singleton → HoldService
//   Nima uchun? IMemoryCache va _activeHolds dictionary
//   dastur bo'yi bir xil bo'lishi kerak.
//   Scoped bo'lsa — har so'rovda yangisi yaratiladi, holdlar yo'qoladi.
//
builder.Services.AddSingleton<IHoldService,          HoldService>();

// Scoped → har HTTP so'rov uchun yangi nusxa
builder.Services.AddScoped<IBookingService,          BookingService>();
builder.Services.AddScoped<IRoomService,             RoomService>();
builder.Services.AddScoped<ICleaningService,         CleaningService>();
builder.Services.AddScoped<IHotelServiceManager,     HotelServiceManager>();
builder.Services.AddScoped<INotificationService,     NotificationService>();
builder.Services.AddScoped<IAuthService,             AuthService>();
builder.Services.AddScoped<IDashboardService,        DashboardService>();
builder.Services.AddScoped<IGuestService,            GuestService>();
builder.Services.AddScoped<IBranchService,           BranchService>();

// ── Background Job ────────────────────────────────────────
//
// ExpiredBookingWorker — har 60 soniyada ishlaydi.
// Muddati o'tgan Requested bronlarni Abandoned qiladi.
// IMemoryCache TTL ning ishonchli zaxira qatlami.
//
builder.Services.AddHostedService<ExpiredBookingWorker>();

// ── Logging ───────────────────────────────────────────────
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// ═════════════════════════════════════════════════════════
var app = builder.Build();
// ═════════════════════════════════════════════════════════

// ── Static files — wwwroot/ ───────────────────────────────
app.UseStaticFiles();

// ── OpenAPI + Scalar UI ───────────────────────────────────
// /openapi/v1.json → OpenAPI spetsifikatsiyasi
// /scalar/v1       → Interaktiv API UI (Swagger o'rniga)
app.MapOpenApi();
app.MapScalarApiReference();

// ── CORS ─────────────────────────────────────────────────
app.UseCors();

// ── Default sahifa ────────────────────────────────────────
app.MapGet("/", ctx =>
{
    ctx.Response.Redirect("/index.html");
    return Task.CompletedTask;
});

// ── Controllers ───────────────────────────────────────────
app.MapControllers();

// ── Boshlash xabari ───────────────────────────────────────
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("\n  🏨  HotelOS — .NET 10\n");
Console.ResetColor();
Console.WriteLine("  🌐  http://localhost:5000");
Console.WriteLine("  📚  http://localhost:5000/scalar/v1   ← API UI");
Console.WriteLine("  📄  http://localhost:5000/openapi/v1.json");
Console.WriteLine("  👤  REC001/admin123 | GST001/guest123 | HK001/hk123\n");

app.Run();
