// Domain/Entities/Address.cs
// Manzil — Value Object (ID si yo'q, qiymatlari muhim)
// DDD: Value Object — o'zgarmas, teng taqqoslanadi
namespace HotelOS.Domain.Entities;

public sealed class Address
{
    public string Street  { get; init; } = "";
    public string City    { get; init; } = "";
    public string Country { get; init; } = "";
}
