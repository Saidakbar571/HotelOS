// Application/Interfaces/IHotelServiceManager.cs
using HotelOS.Domain.Entities;

namespace HotelOS.Application.Interfaces;

public interface IHotelServiceManager
{
    (bool Success, string Message) Add(string bookingId, string type, string description, decimal cost);
    List<HotelService> GetByBooking(string bookingId);
}
