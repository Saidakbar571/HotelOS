// Application/Services/NotificationService.cs
using HotelOS.Application.Interfaces;
using HotelOS.Infrastructure.Persistence;

namespace HotelOS.Application.Services;

// Haqiqiy tizimda: SendGrid (email), Eskiz (SMS)
public sealed class NotificationService : INotificationService
{
    public void Notify(string guestId, string message)
    {
        var guest = AppDatabase.FindGuest(guestId);
        if (guest == null) return;
        Console.WriteLine($"[EMAIL → {guest.Email}]: {message}");
        Console.WriteLine($"[SMS   → {guest.Phone}]: {message}");
    }
}
