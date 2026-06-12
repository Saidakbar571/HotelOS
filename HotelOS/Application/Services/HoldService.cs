// Application/Services/HoldService.cs
//
// Xonani 10 daqiqa vaqtincha ushlab turish mexanizmi.
//
// IMemoryCache qanday ishlaydi?
//   - Set(key, value, TTL) → ma'lumotni xotirada saqlaydi
//   - TTL = 10 daqiqa → o'tsa, avtomatik o'chadi
//   - EvictionCallback → o'chganda xonani bo'shatadi, bronni Abandoned qiladi
//
// Nima uchun Singleton?
//   _activeHolds dictionary va IMemoryCache dastur bo'yi bir xil bo'lishi kerak.
//   Scoped bo'lsa — har so'rovda yangisi yaratiladi va holdlar yo'qoladi.
//
// Thread-safety:
//   lock(_lock) — bir vaqtda bir nechta so'rov kelsa ham xavfsiz ishlaydi.
using HotelOS.Application.Interfaces;
using HotelOS.Domain.Enums;
using HotelOS.Infrastructure.Persistence;
using Microsoft.Extensions.Caching.Memory;

namespace HotelOS.Application.Services;

public sealed class HoldService : IHoldService
{
    private readonly IMemoryCache _cache;
    private readonly Dictionary<string, string> _activeHolds = new(); // holdId → roomNumber
    private readonly Lock _lock = new();

    public HoldService(IMemoryCache cache) => _cache = cache;

    public int ActiveCount { get { lock (_lock) return _activeHolds.Count; } }

    // Xonani 10 daqiqa band qilish
    // Qaytadi: holdId — to'lov qadamida ishlatiladi
    public string CreateHold(string roomNumber)
    {
        var room = AppDatabase.FindRoom(roomNumber);
        if (room == null || !room.IsAvailable())
            return "";

        var holdId = "HOLD-" + Guid.NewGuid().ToString("N")[..6].ToUpper();

        var options = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(10))
            .RegisterPostEvictionCallback(OnExpired);

        lock (_lock)
        {
            _cache.Set(holdId, roomNumber, options);
            _activeHolds[holdId] = roomNumber;
            room.Status = RoomStatus.Reserved;
        }

        Console.WriteLine($"[HOLD] ✅ {holdId} | Xona {roomNumber} | 10 daqiqa band.");
        return holdId;
    }

    public bool IsActive(string holdId) => _cache.TryGetValue(holdId, out _);

    // To'lov OK → cache dan o'chirish
    // Remove() chaqirilsa EvictionReason = Removed → OnExpired ISHLAMAYDI (to'g'ri)
    public void Confirm(string holdId)
    {
        lock (_lock)
        {
            _cache.Remove(holdId);
            _activeHolds.Remove(holdId);
        }
        Console.WriteLine($"[HOLD] ✅ {holdId} tasdiqlandi.");
    }

    // Bekor qilish → xona bo'shaydi
    public void Release(string holdId)
    {
        lock (_lock)
        {
            if (!_activeHolds.TryGetValue(holdId, out var roomNumber)) return;
            var room = AppDatabase.FindRoom(roomNumber);
            if (room != null) room.Status = RoomStatus.Available;
            _cache.Remove(holdId);
            _activeHolds.Remove(holdId);
        }
        Console.WriteLine($"[HOLD] 🔓 {holdId} bekor qilindi.");
    }

    // 10 daqiqa o'tdi → bu callback avtomatik chaqiriladi
    private void OnExpired(object key, object? value, EvictionReason reason, object? state)
    {
        // Faqat TTL tugaganda → reason = Expired
        // Confirm() / Release() da → reason = Removed → bu blok ISHLAMAYDI
        if (reason != EvictionReason.Expired) return;

        var holdId     = key.ToString()!;
        var roomNumber = value?.ToString() ?? "";

        Console.WriteLine($"[HOLD] ⏰ {holdId} — 10 daqiqa tugadi. Xona {roomNumber} bo'shaydi.");

        var room = AppDatabase.FindRoom(roomNumber);
        if (room is { Status: RoomStatus.Reserved })
            room.Status = RoomStatus.Available;

        var booking = AppDatabase.Bookings.FirstOrDefault(b =>
            b.Room?.RoomNumber == roomNumber &&
            b.Status == BookingStatus.Requested);

        if (booking != null)
        {
            booking.Status = BookingStatus.Abandoned;
            Console.WriteLine($"[HOLD] ⏰ Bron {booking.Id} → Abandoned.");
        }

        lock (_lock) { _activeHolds.Remove(holdId); }
    }
}
