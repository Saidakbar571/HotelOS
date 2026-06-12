// Domain/Entities/Payment.cs
// To'lov — haqiqiy tizimda Stripe / Payme API bu yerda chaqiriladi
using HotelOS.Domain.Enums;

namespace HotelOS.Domain.Entities;

public sealed class Payment
{
    public string        Id        { get; init; } = "PAY-" + Guid.NewGuid().ToString("N")[..6].ToUpper();
    public string        BookingId { get; init; } = "";
    public decimal       Amount    { get; init; }
    public string        Method    { get; init; } = ""; // cash | credit | check
    public string        Detail    { get; init; } = "";
    public PaymentStatus Status    { get; set;  } = PaymentStatus.Pending;

    // Haqiqiy tizimda: payment gateway ga so'rov yuborish
    public void Process() => Status = PaymentStatus.Completed;
}
