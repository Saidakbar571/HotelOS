// Domain/Entities/Person.cs
//
// DDD: abstract Person — umumiy xususiyatlar
// Guest, Receptionist, HouseKeeper — alohida rollarga ega foydalanuvchilar
//
// Nima uchun bitta faylda?
//   Uchala klass juda kichik (1-2 qator) va bitta kontekstga tegishli.
//   Alohida fayllarga bo'lish ortiqcha murakkablik yaratadi (KISS).
using HotelOS.Domain.Enums;

namespace HotelOS.Domain.Entities;

public abstract class Person
{
    public string   Name    { get; set; } = "";
    public string   Email   { get; set; } = "";
    public string   Phone   { get; set; } = "";
    public Address  Address { get; set; } = new();
    public Account  Account { get; set; } = new();
    public UserRole Role    { get; protected set; }
}

public sealed class Guest        : Person { public Guest()        => Role = UserRole.Guest;        }
public sealed class Receptionist : Person { public Receptionist() => Role = UserRole.Receptionist; }
public sealed class HouseKeeper  : Person { public HouseKeeper()  => Role = UserRole.HouseKeeper;  }
