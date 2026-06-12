// Application/Services/HotelServiceManager.cs
using HotelOS.Application.Interfaces;
using HotelOS.Domain.Entities;
using HotelOS.Domain.Enums;
using HotelOS.Infrastructure.Persistence;

namespace HotelOS.Application.Services;

public sealed class HotelServiceManager : IHotelServiceManager
{
    public (bool Success, string Message) Add(
        string bookingId, string type, string description, decimal cost)
    {
        var booking = AppDatabase.FindBooking(bookingId);
        if (booking == null)
            return (false, "Bron topilmadi.");

        if (booking.Status != BookingStatus.Confirmed)
            return (false, "Faqat Confirmed bronlarga xizmat qo'shiladi.");

        var service = new HotelService
        {
            BookingId   = bookingId,
            RoomNumber  = booking.Room?.RoomNumber ?? "",
            Type        = type,
            Description = description,
            Cost        = cost
        };
        AppDatabase.HotelServices.Add(service);

        if (booking.Invoice != null)
            booking.Invoice.Amount += cost;

        return (true, $"Xizmat qo'shildi: {description} — {cost:F2} USD.");
    }

    public List<HotelService> GetByBooking(string bookingId)
        => AppDatabase.HotelServices.Where(s => s.BookingId == bookingId).ToList();
}
