// Application/Interfaces/INotificationService.cs
namespace HotelOS.Application.Interfaces;

public interface INotificationService
{
    void Notify(string guestId, string message);
}
