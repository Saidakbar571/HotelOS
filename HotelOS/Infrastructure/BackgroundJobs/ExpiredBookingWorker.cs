// Infrastructure/BackgroundJobs/ExpiredBookingWorker.cs
//
// Background Job — har 60 soniyada ishlaydi.
//
// Nima qiladi?
//   DB dagi Requested holati 10 daqiqadan oshgan bronlarni Abandoned qiladi.
//
// Nima uchun kerak? (IMemoryCache TTL bo'lsa ham)
//   IMemoryCache server xotirasida — restart bo'lsa yo'qoladi.
//   Worker esa DB dagi "yetim" Requested bronlarni tozalaydi.
//
// Bu ikki mexanizm birga ishlaydi:
//   Cache TTL  → tezkor, real-time (ms da)
//   Worker     → ishonchli, zaxira (60s da)
//
// SOLID → SRP: Faqat bitta ish — muddati o'tgan bronlarni topib Abandoned qilish.
using HotelOS.Domain.Enums;
using HotelOS.Infrastructure.Persistence;

namespace HotelOS.Infrastructure.BackgroundJobs;

public sealed class ExpiredBookingWorker : BackgroundService
{
    private readonly ILogger<ExpiredBookingWorker> _logger;

    private static readonly TimeSpan HoldDuration  = TimeSpan.FromMinutes(10);
    private static readonly TimeSpan CheckInterval = TimeSpan.FromSeconds(60);

    public ExpiredBookingWorker(ILogger<ExpiredBookingWorker> logger)
        => _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[Worker] ✅ ExpiredBookingWorker ishga tushdi.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                AbandonExpiredBookings();
            }
            catch (Exception ex)
            {
                // Xato bo'lsa worker to'xtamaydi — faqat loglaydi
                _logger.LogError(ex, "[Worker] ❌ Xato.");
            }

            await Task.Delay(CheckInterval, stoppingToken);
        }
    }

    private void AbandonExpiredBookings()
    {
        // ToList() — iteratsiya paytida ro'yxat o'zgarmasin
        var expired = AppDatabase.Bookings
            .Where(b =>
                b.Status == BookingStatus.Requested &&
                DateTime.UtcNow - b.CreatedAt > HoldDuration)
            .ToList();

        if (!expired.Any()) return;

        _logger.LogInformation("[Worker] 🔍 {Count} ta muddati o'tgan bron.", expired.Count);

        foreach (var booking in expired)
        {
            booking.Status = BookingStatus.Abandoned;

            if (booking.Room is { Status: RoomStatus.Reserved })
                booking.Room.Status = RoomStatus.Available;

            _logger.LogWarning(
                "[Worker] ⏰ {Id} → Abandoned. Xona {Room} bo'shadi.",
                booking.Id,
                booking.Room?.RoomNumber ?? "?");
        }
    }
}
