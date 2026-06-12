// Domain/Entities/RoomKey.cs
// R9: Har xonaning o'z kaliti bor, master kalit ham mavjud
namespace HotelOS.Domain.Entities;

public sealed class RoomKey
{
    public string Id       { get; init; } = "KEY-" + Guid.NewGuid().ToString("N")[..6].ToUpper();
    public string Barcode  { get; set;  } = "";
    public bool   IsActive { get; set;  } = true;
    public bool   IsMaster { get; init; } = false;
}
