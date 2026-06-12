// Application/Services/CleaningService.cs
using HotelOS.Application.Interfaces;
using HotelOS.Domain.Entities;
using HotelOS.Domain.Enums;
using HotelOS.Infrastructure.Persistence;

namespace HotelOS.Application.Services;

public sealed class CleaningService : ICleaningService
{
    public List<CleaningTask> GetAll() => AppDatabase.CleaningTasks;

    public (bool Success, string Message) Complete(string taskId)
    {
        var task = AppDatabase.CleaningTasks.FirstOrDefault(t => t.Id == taskId);
        if (task == null) return (false, "Vazifa topilmadi.");

        task.IsCompleted = true;

        var room = AppDatabase.FindRoom(task.RoomNumber);
        if (room != null) room.Status = RoomStatus.Available;

        Console.WriteLine($"[CLEANING] ✅ Xona {task.RoomNumber} tozalandi → Available.");
        return (true, $"Xona {task.RoomNumber} tozalandi, endi bo'sh.");
    }

    // Check-out dan keyin avtomatik chaqiriladi
    public void CreateAfterCheckout(string roomNumber)
    {
        var housekeeper = AppDatabase.HouseKeepers.FirstOrDefault();

        AppDatabase.CleaningTasks.Add(new CleaningTask
        {
            RoomNumber      = roomNumber,
            Description     = "Check-out dan keyin standart tozalash",
            DurationMinutes = 30,
            HousekeeperId   = housekeeper?.Account.Id ?? "",
            HousekeeperName = housekeeper?.Name ?? ""
        });

        var room = AppDatabase.FindRoom(roomNumber);
        if (room != null) room.Status = RoomStatus.BeingServiced;
    }
}
