// Application/Services/DashboardService.cs
// SOLID SRP: faqat dashboard statistikasini yig'adi
using HotelOS.Application.Interfaces;
using HotelOS.Domain.Enums;
using HotelOS.Infrastructure.Persistence;

namespace HotelOS.Application.Services;

public sealed class DashboardService : IDashboardService
{
    private readonly IHoldService _holds;

    public DashboardService(IHoldService holds) => _holds = holds;

    public object GetStats() => new
    {
        totalRooms    = AppDatabase.Rooms.Count,
        available     = AppDatabase.Rooms.Count(r => r.Status == RoomStatus.Available),
        occupied      = AppDatabase.Rooms.Count(r => r.Status == RoomStatus.Occupied),
        reserved      = AppDatabase.Rooms.Count(r => r.Status == RoomStatus.Reserved),
        beingServiced = AppDatabase.Rooms.Count(r => r.Status == RoomStatus.BeingServiced),
        totalBookings = AppDatabase.Bookings.Count,
        confirmed     = AppDatabase.Bookings.Count(b => b.Status == BookingStatus.Confirmed),
        requested     = AppDatabase.Bookings.Count(b => b.Status == BookingStatus.Requested),
        abandoned     = AppDatabase.Bookings.Count(b => b.Status == BookingStatus.Abandoned),
        completed     = AppDatabase.Bookings.Count(b => b.Status == BookingStatus.Completed),
        pendingTasks  = AppDatabase.CleaningTasks.Count(t => !t.IsCompleted),
        totalRevenue  = AppDatabase.Invoices.Sum(i => i.Amount),
        activeHolds   = _holds.ActiveCount
    };
}
