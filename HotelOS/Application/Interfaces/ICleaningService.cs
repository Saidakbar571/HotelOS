// Application/Interfaces/ICleaningService.cs
using HotelOS.Domain.Entities;

namespace HotelOS.Application.Interfaces;

public interface ICleaningService
{
    List<CleaningTask> GetAll();
    (bool Success, string Message) Complete(string taskId);
    void CreateAfterCheckout(string roomNumber);
}
