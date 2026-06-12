// Application/Services/BranchService.cs
// SOLID SRP: faqat filiallar bilan ishlaydi
using HotelOS.Application.Interfaces;
using HotelOS.Infrastructure.Persistence;

namespace HotelOS.Application.Services;

public sealed class BranchService : IBranchService
{
    public IEnumerable<object> GetAll() =>
        AppDatabase.Branches.Select(b => new
        {
            b.Id,
            b.Name,
            city      = b.Address.City,
            roomCount = b.Rooms.Count,
            available = b.Rooms.Count(r => r.IsAvailable())
        });
}
