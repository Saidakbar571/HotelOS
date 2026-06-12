// Domain/Entities/Booking.cs
//
// Bron — tizimning markaziy entity si
//
// Hayot sikli:
//   Requested  →  Confirmed   (to'lov OK, 10 daqiqa ichida)
//   Requested  →  Abandoned   (10 daqiqa o'tdi YOKI to'lov FAIL)
//   Confirmed  →  Canceled    (mehmon bekor qildi, R5)
//   Confirmed  →  Completed   (check-out qilindi)
using HotelOS.Domain.Enums;

namespace HotelOS.Domain.Entities;

public sealed class Booking
{
    public string        Id            { get; init; } = "RES-" + Guid.NewGuid().ToString("N")[..6].ToUpper();
    public string        GuestId       { get; init; } = "";
    public Room?         Room          { get; init; }
    public DateTime      CheckInDate   { get; init; }
    public DateTime      CheckOutDate  { get; init; }
    public int           DurationDays  { get; init; }
    public decimal       AdvanceAmount { get; set;  }
    public BookingStatus Status        { get; set;  } = BookingStatus.Requested;
    public DateTime      CreatedAt     { get; init; } = DateTime.UtcNow;
    public Invoice?      Invoice       { get; set;  }

    // R5: Kirish vaqtiga 24+ soat qolgan bo'lsa refund bor
    public bool IsRefundable() => (CheckInDate - DateTime.UtcNow).TotalHours >= 24;
}
