// Domain/Entities/Branch.cs
// R10: Ko'p filial — mehmonxonaning har bir joylashuvi
namespace HotelOS.Domain.Entities;

public sealed class Branch
{
    public string     Id      { get; init; } = Guid.NewGuid().ToString("N")[..6].ToUpper();
    public string     Name    { get; init; } = "";
    public Address    Address { get; init; } = new();
    public List<Room> Rooms   { get; init; } = new();
}
