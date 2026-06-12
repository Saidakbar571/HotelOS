// Application/Services/AuthService.cs
// SOLID SRP: faqat autentifikatsiya — login
using HotelOS.Application.Interfaces;
using HotelOS.Infrastructure.Persistence;

namespace HotelOS.Application.Services;

public sealed class AuthService : IAuthService
{
    public (bool Success, string Role, string AccountId, string Name)
        Login(string accountId, string password)
    {
        var (person, role) = AppDatabase.FindUser(accountId, password);
        if (person == null) return (false, "", "", "");
        return (true, role, person.Account.Id, person.Name);
    }

    public (bool Success, string Message, string AccountId)
        Register(string name, string email, string phone, string password)
    {
        // Delegatsiya — GuestService ga yo'naltiriladi
        var guestService = new GuestService();
        return guestService.Register(name, email, phone, password);
    }
}
