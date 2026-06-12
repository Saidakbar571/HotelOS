// Domain/Enums/BookingStatus.cs
// Bron hayot sikli:
//   Requested → Confirmed  (to'lov OK)
//   Requested → Abandoned  (10 daqiqa o'tdi yoki to'lov FAIL)
//   Confirmed → Canceled   (mehmon bekor qildi)
//   Confirmed → Completed  (check-out qilindi) ← to'g'ri nom
namespace HotelOS.Domain.Enums;

public enum BookingStatus
{
    Requested,  // Hold qilindi, to'lov kutilmoqda
    Confirmed,  // To'lov OK, bron tasdiqlandi
    Canceled,   // Mehmon bekor qildi
    Abandoned,  // 10 daqiqa o'tdi yoki to'lov muvaffaqiyatsiz
    Completed   // Check-out qilindi — yakunlandi
}
