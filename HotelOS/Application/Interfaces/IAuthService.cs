// Application/Interfaces/IAuthService.cs
namespace HotelOS.Application.Interfaces;

public interface IAuthService
{
    (bool Success, string Role, string AccountId, string Name)
        Login(string accountId, string password);

    (bool Success, string Message, string AccountId)
        Register(string name, string email, string phone, string password);
}
