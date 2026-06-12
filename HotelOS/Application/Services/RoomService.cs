// Application/Services/RoomService.cs
using HotelOS.Application.Interfaces;
using HotelOS.Domain.Entities;
using HotelOS.Domain.Enums;
using HotelOS.Infrastructure.Persistence;

namespace HotelOS.Application.Services;

public sealed class RoomService : IRoomService
{
    public List<Room> GetAll() => AppDatabase.Rooms;

    public List<Room> Search(RoomStyle style, DateTime checkIn, int days)
        => AppDatabase.Rooms.Where(r => r.Style == style && r.IsAvailable()).ToList();

    public (bool Success, string Message) Add(
        string number, RoomStyle style, decimal price, bool smoking, string branchId)
    {
        if (AppDatabase.FindRoom(number) != null)
            return (false, $"Xona #{number} allaqachon mavjud.");

        var room = new Room
        {
            RoomNumber    = number,
            Style         = style,
            PricePerNight = price,
            IsSmoking     = smoking,
            BranchId      = branchId
        };

        AppDatabase.Branches.FirstOrDefault(b => b.Id == branchId)?.Rooms.Add(room);
        AppDatabase.Rooms.Add(room);
        return (true, $"Xona #{number} qo'shildi.");
    }

    public (bool Success, string Message) UpdateStatus(string roomNumber, RoomStatus newStatus)
    {
        var room = AppDatabase.FindRoom(roomNumber);
        if (room == null) return (false, "Xona topilmadi.");
        room.Status = newStatus;
        return (true, $"Xona #{roomNumber} → {newStatus}.");
    }
}
