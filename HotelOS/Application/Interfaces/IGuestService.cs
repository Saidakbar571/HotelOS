// Application/Interfaces/IGuestService.cs
namespace HotelOS.Application.Interfaces;

public interface IGuestService
{
    IEnumerable<object> GetAll();
    (bool Success, string Message, string AccountId) Register(
        string name, string email, string phone, string password);
}
