// Domain/Entities/HotelService.cs
// Mehmon so'ragan qo'shimcha xizmatlar (R8)
// ESLATMA: Bu klass nomi "HotelService" — "RoomService" emas.
// Chunki Application qatlamida IRoomService interfeysi bor,
// nomlar to'qnashmasligi uchun alohida nom berildi.
namespace HotelOS.Domain.Entities;

public sealed class HotelService
{
    public string   Id          { get; init; } = Guid.NewGuid().ToString("N")[..8];
    public string   BookingId   { get; init; } = "";
    public string   RoomNumber  { get; init; } = "";
    public string   Type        { get; init; } = ""; // room | kitchen | amenity
    public string   Description { get; init; } = "";
    public decimal  Cost        { get; init; }
    public DateTime CreatedAt   { get; init; } = DateTime.UtcNow;
}
