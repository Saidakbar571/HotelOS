// Application/Services/GuestService.cs
// SOLID SRP: faqat mehmonlar bilan ishlaydi
using HotelOS.Application.Interfaces;
using HotelOS.Domain.Entities;
using HotelOS.Infrastructure.Persistence;

namespace HotelOS.Application.Services;

public sealed class GuestService : IGuestService
{
    public IEnumerable<object> GetAll() =>
        AppDatabase.Guests.Select(g => new
        {
            id     = g.Account.Id,
            g.Name,
            g.Email,
            g.Phone,
            status = g.Account.Status.ToString()
        });

    public (bool Success, string Message, string AccountId)
        Register(string name, string email, string phone, string password)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password))
            return (false, "Ism va parol majburiy.", "");

        if (AppDatabase.Guests.Any(g => g.Email == email))
            return (false, "Bu email allaqachon ro'yxatdan o'tgan.", "");

        var guest = new Guest
        {
            Name    = name,
            Email   = email,
            Phone   = phone,
            Account = new Account { Password = password }
        };
        AppDatabase.Guests.Add(guest);
        return (true, "Ro'yxatdan o'tdingiz!", guest.Account.Id);
    }
}
