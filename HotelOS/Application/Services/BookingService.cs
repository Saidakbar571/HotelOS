// Application/Services/BookingService.cs
//
// Bron oqimi — 2 qadam:
//   1. HoldRoomAsync      → xona 10 daqiqa Reserved, bron Requested
//   2. ConfirmPaymentAsync → to'lov → Confirmed yoki Abandoned
//
// SOLID SRP : Faqat bron logikasi.
// SOLID DIP : IHoldService, INotificationService, ICleaningService orqali.
// KISS      : Har metod bir ish qiladi.
using HotelOS.Application.Interfaces;
using HotelOS.Domain.Entities;
using HotelOS.Domain.Enums;
using HotelOS.Infrastructure.Persistence;

namespace HotelOS.Application.Services;

public sealed class BookingService : IBookingService
{
    private readonly IHoldService         _holds;
    private readonly INotificationService _notifications;
    private readonly ICleaningService     _cleaning;

    public BookingService(
        IHoldService         holds,
        INotificationService notifications,
        ICleaningService     cleaning)
    {
        _holds         = holds;
        _notifications = notifications;
        _cleaning      = cleaning;
    }

    // So'rov metodlari
    public IEnumerable<object> GetAll() =>
        AppDatabase.Bookings.Select(b => new
        {
            id       = b.Id,
            b.GuestId,
            room     = b.Room?.RoomNumber,
            checkIn  = b.CheckInDate.ToString("yyyy-MM-dd"),
            checkOut = b.CheckOutDate.ToString("yyyy-MM-dd"),
            status   = b.Status.ToString(),
            b.AdvanceAmount,
            total    = b.Invoice?.Amount
        });

    public object? GetOne(string bookingId)
    {
        var b = AppDatabase.FindBooking(bookingId);
        if (b == null) return null;

        var services = AppDatabase.HotelServices
            .Where(s => s.BookingId == bookingId)
            .Select(s => new { s.Type, s.Description, s.Cost });

        return new
        {
            bookingId = b.Id,
            b.GuestId,
            room     = b.Room?.RoomNumber,
            style    = b.Room?.Style.ToString(),
            checkIn  = b.CheckInDate.ToString("yyyy-MM-dd"),
            checkOut = b.CheckOutDate.ToString("yyyy-MM-dd"),
            status   = b.Status.ToString(),
            b.AdvanceAmount,
            total    = b.Invoice?.Amount,
            services
        };
    }

    // ----------------------------------------------------------
    // Qadam 1: Xonani 10 daqiqa band qilish → Bron = Requested
    // ----------------------------------------------------------
    public Task<(bool Success, string Message, string BookingId, string HoldId)>
        HoldRoomAsync(string guestId, string roomNumber, DateTime checkIn, int days, decimal advance)
    {
        if (days < 1)
            return Task.FromResult((false, "Kamida 1 kun bo'lishi kerak.", "", ""));

        var room = AppDatabase.FindRoom(roomNumber);
        if (room == null)
            return Task.FromResult((false, $"Xona {roomNumber} topilmadi.", "", ""));

        if (!room.IsAvailable())
            return Task.FromResult((false, $"Xona {roomNumber} hozir band.", "", ""));

        var holdId = _holds.CreateHold(roomNumber);
        if (string.IsNullOrEmpty(holdId))
            return Task.FromResult((false, "Xonani band qilish muvaffaqiyatsiz.", "", ""));

        var booking = new Booking
        {
            GuestId       = guestId,
            Room          = room,
            CheckInDate   = checkIn,
            CheckOutDate  = checkIn.AddDays(days),
            DurationDays  = days,
            AdvanceAmount = advance,
            Status        = BookingStatus.Requested,
            CreatedAt     = DateTime.UtcNow
        };
        AppDatabase.Bookings.Add(booking);

        Console.WriteLine($"[BOOKING] Qadam 1 ✅ | {booking.Id} | Xona {roomNumber} | 10 daqiqa bor.");
        return Task.FromResult((true, $"Xona {roomNumber} 10 daqiqaga band qilindi.", booking.Id, holdId));
    }

    // ----------------------------------------------------------
    // Qadam 2: To'lovni tasdiqlash → Confirmed yoki Abandoned
    // ----------------------------------------------------------
    public Task<(bool Success, string Message)>
        ConfirmPaymentAsync(string bookingId, string holdId, string method, string detail, decimal amount)
    {
        // Hold hali aktivmi? 10 daqiqa o'tmadimi?
        if (!_holds.IsActive(holdId))
        {
            var expired = AppDatabase.FindBooking(bookingId);
            if (expired != null) expired.Status = BookingStatus.Abandoned;
            return Task.FromResult((false, "10 daqiqa muddati o'tib ketdi. Qaytadan xona tanlang."));
        }

        var booking = AppDatabase.FindBooking(bookingId);
        if (booking == null)
            return Task.FromResult((false, "Bron topilmadi."));

        if (booking.Status != BookingStatus.Requested)
            return Task.FromResult((false, $"Bron holati noto'g'ri: {booking.Status}."));

        var payment = new Payment
        {
            BookingId = bookingId,
            Amount    = amount,
            Method    = method,
            Detail    = detail
        };
        payment.Process(); // Haqiqiy tizimda: Stripe / Payme API
        AppDatabase.Payments.Add(payment);

        if (payment.Status == PaymentStatus.Completed)
        {
            _holds.Confirm(holdId);

            booking.Status        = BookingStatus.Confirmed;
            booking.AdvanceAmount = amount;
            booking.Invoice       = new Invoice { BookingId = bookingId, Amount = amount };
            AppDatabase.Invoices.Add(booking.Invoice);

            _notifications.Notify(booking.GuestId,
                $"✅ Bron tasdiqlandi! {booking.Id} | " +
                $"Kirish: {booking.CheckInDate:dd.MM.yyyy} | Xona: {booking.Room?.RoomNumber}.");

            Console.WriteLine($"[BOOKING] Qadam 2 ✅ | {booking.Id} → Confirmed.");
            return Task.FromResult((true, "To'lov qabul qilindi. Bron tasdiqlandi!"));
        }

        _holds.Release(holdId);
        booking.Status = BookingStatus.Abandoned;
        Console.WriteLine($"[BOOKING] Qadam 2 ❌ | {booking.Id} → Abandoned.");
        return Task.FromResult((false, "To'lov amalga oshmadi. Bron bekor qilindi."));
    }

    // R5: Bekor qilish — 24 soatdan oldin refund bor
    public Task<(bool Success, string Message)> CancelAsync(string bookingId)
    {
        var booking = AppDatabase.FindBooking(bookingId);
        if (booking == null)
            return Task.FromResult((false, "Bron topilmadi."));

        if (booking.Status is BookingStatus.Canceled or BookingStatus.Abandoned or BookingStatus.Completed)
            return Task.FromResult((false, $"Bu bron allaqachon yakunlangan: {booking.Status}."));

        booking.Status = BookingStatus.Canceled;
        if (booking.Room != null)
            booking.Room.Status = RoomStatus.Available;

        var message = booking.IsRefundable()
            ? $"Bekor qilindi. Refund: {booking.AdvanceAmount:F2} USD — 3-5 ish kunida."
            : "Bekor qilindi. 24 soatdan kam qolganligi uchun refund yo'q.";

        _notifications.Notify(booking.GuestId, message);
        return Task.FromResult((true, message));
    }

    // Check-in
    public Task<(bool Success, string Message)> CheckInAsync(string bookingId)
    {
        var booking = AppDatabase.FindBooking(bookingId);
        if (booking == null)
            return Task.FromResult((false, "Bron topilmadi."));
        if (booking.Room == null)
            return Task.FromResult((false, "Bronga xona tayinlanmagan."));
        if (booking.Status != BookingStatus.Confirmed)
            return Task.FromResult((false, $"Faqat Confirmed bronlar uchun check-in. Holat: {booking.Status}."));
        if (!booking.Room.CheckIn())
            return Task.FromResult((false, "Xona hozir band."));

        // R9: Kalit yaratish
        var key = new RoomKey { Barcode = $"KEY-{booking.Room.RoomNumber}-{DateTime.UtcNow:HHmmss}" };
        booking.Room.Keys.Add(key);

        _notifications.Notify(booking.GuestId,
            $"Xush kelibsiz! Xona {booking.Room.RoomNumber}. Kalit: {key.Id}.");

        Console.WriteLine($"[CHECK-IN] ✅ {bookingId} | Xona: {booking.Room.RoomNumber}.");
        return Task.FromResult((true, $"Check-in muvaffaqiyatli. Kalit: {key.Id}."));
    }

    // Check-out
    public Task<(bool Success, string Message, decimal Total)> CheckOutAsync(string bookingId)
    {
        var booking = AppDatabase.FindBooking(bookingId);
        if (booking == null)
            return Task.FromResult((false, "Bron topilmadi.", 0m));
        if (booking.Room == null)
            return Task.FromResult((false, "Xona topilmadi.", 0m));

        var servicesTotal = AppDatabase.HotelServices
            .Where(s => s.BookingId == bookingId)
            .Sum(s => s.Cost);

        var roomTotal  = booking.Room.PricePerNight * booking.DurationDays;
        var grandTotal = Math.Max(0, roomTotal + servicesTotal - booking.AdvanceAmount);

        booking.Room.CheckOut();
        booking.Status = BookingStatus.Completed;

        if (booking.Invoice != null)
            booking.Invoice.Amount = grandTotal;

        // R7: Tozalash vazifasi avtomatik yaratiladi
        _cleaning.CreateAfterCheckout(booking.Room.RoomNumber);

        _notifications.Notify(booking.GuestId,
            $"Check-out. Xona: {roomTotal:F2} | Xizmat: {servicesTotal:F2} | " +
            $"Avans: -{booking.AdvanceAmount:F2} | Jami: {grandTotal:F2} USD.");

        Console.WriteLine($"[CHECK-OUT] ✅ {bookingId} | Jami: {grandTotal:F2} USD.");
        return Task.FromResult((true, "Check-out muvaffaqiyatli.", grandTotal));
    }
}
