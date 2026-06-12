// Application/Interfaces/IHoldService.cs
// Hold mexanizmi — xonani vaqtincha ushlab turish
namespace HotelOS.Application.Interfaces;

public interface IHoldService
{
    // Xonani 10 daqiqa band qilish, holdId qaytaradi
    string CreateHold(string roomNumber);

    // Hold hali aktivmi? (10 daqiqa o'tmadimi?)
    bool IsActive(string holdId);

    // To'lov OK → hold yopiladi (cache dan o'chiriladi)
    void Confirm(string holdId);

    // Bekor qilish → hold yopiladi, xona bo'shaydi
    void Release(string holdId);

    // Hozir nechta faol hold bor?
    int ActiveCount { get; }
}
