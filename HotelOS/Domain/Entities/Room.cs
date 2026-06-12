// Domain/Entities/Room.cs
// Xona — tizimning asosiy resursi
// DDD Entity: o'z ID si bor, holati o'zgarishi mumkin
using HotelOS.Domain.Enums;

namespace HotelOS.Domain.Entities;

public sealed class Room
{
    public string        RoomNumber    { get; init; } = "";
    public RoomStyle     Style         { get; init; }
    public RoomStatus    Status        { get; set;  } = RoomStatus.Available;
    public decimal       PricePerNight { get; init; }
    public bool          IsSmoking     { get; init; }
    public string        BranchId      { get; init; } = "";
    public List<RoomKey> Keys          { get; init; } = new();

    public bool IsAvailable() => Status == RoomStatus.Available;

    // Mehmon kirdi → Occupied
    public bool CheckIn()
    {
        if (!IsAvailable()) return false;
        Status = RoomStatus.Occupied;
        return true;
    }

    // Mehmon chiqdi → tozalanishi kerak
    public void CheckOut() => Status = RoomStatus.BeingServiced;
}
