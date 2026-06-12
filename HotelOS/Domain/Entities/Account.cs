// Domain/Entities/Account.cs
// Foydalanuvchi login ma'lumotlari
using HotelOS.Domain.Enums;

namespace HotelOS.Domain.Entities;

public sealed class Account
{
    public string        Id       { get; init; } = NewId();
    public string        Password { get; set;  } = "";
    public AccountStatus Status   { get; set;  } = AccountStatus.Active;

    public bool ResetPassword(string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword)) return false;
        Password = newPassword;
        return true;
    }

    private static string NewId() => Guid.NewGuid().ToString("N")[..8].ToUpper();
}
