// Domain/Entities/CleaningTask.cs
// Tozalash vazifasi (R7)
namespace HotelOS.Domain.Entities;

public sealed class CleaningTask
{
    public string   Id              { get; init; } = Guid.NewGuid().ToString("N")[..8];
    public string   RoomNumber      { get; init; } = "";
    public string   Description     { get; init; } = "";
    public DateTime CreatedAt       { get; init; } = DateTime.UtcNow;
    public int      DurationMinutes { get; init; }
    public string   HousekeeperId   { get; init; } = "";
    public string   HousekeeperName { get; init; } = "";
    public bool     IsCompleted     { get; set;  } = false;
}
