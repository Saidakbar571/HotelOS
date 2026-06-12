// Domain/Entities/Invoice.cs
// Hisob-faktura — bron uchun to'liq moliyaviy hujjat
namespace HotelOS.Domain.Entities;

public sealed class Invoice
{
    public string  Id        { get; init; } = "INV-" + Guid.NewGuid().ToString("N")[..6].ToUpper();
    public string  BookingId { get; init; } = "";
    public decimal Amount    { get; set;  }
}
