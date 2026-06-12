// Application/Interfaces/IBookingService.cs
namespace HotelOS.Application.Interfaces;

public interface IBookingService
{
    // So'rov metodlari
    IEnumerable<object> GetAll();
    object? GetOne(string bookingId);

    // Qadam 1: Xonani 10 daqiqa band qil → Bron = Requested
    Task<(bool Success, string Message, string BookingId, string HoldId)>
        HoldRoomAsync(string guestId, string roomNumber, DateTime checkIn, int days, decimal advance);

    // Qadam 2: To'lovni tasdiqlash → Bron = Confirmed yoki Abandoned
    Task<(bool Success, string Message)>
        ConfirmPaymentAsync(string bookingId, string holdId, string method, string detail, decimal amount);

    // R5: Bekor qilish — 24 soatdan oldin refund bor
    Task<(bool Success, string Message)> CancelAsync(string bookingId);

    Task<(bool Success, string Message)>                CheckInAsync(string bookingId);
    Task<(bool Success, string Message, decimal Total)> CheckOutAsync(string bookingId);
}
