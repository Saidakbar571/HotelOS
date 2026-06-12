// Application/Interfaces/IRoomService.cs
using HotelOS.Domain.Entities;
using HotelOS.Domain.Enums;

namespace HotelOS.Application.Interfaces;

public interface IRoomService
{
    List<Room> GetAll();
    List<Room> Search(RoomStyle style, DateTime checkIn, int days);
    (bool Success, string Message) Add(string number, RoomStyle style, decimal price, bool smoking, string branchId);
    (bool Success, string Message) UpdateStatus(string roomNumber, RoomStatus newStatus);
}
