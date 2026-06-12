// Application/DTOs/Requests.cs
// API so'rov ob'ektlari — faqat ma'lumot tashuvchi (DTO)
namespace HotelOS.Application.DTOs;

public record LoginRequest   (string AccountId, string Password);
public record RegisterRequest(string Name, string Email, string Phone, string Password);

public record HoldRequest(
    string  GuestId,
    string  RoomNumber,
    string  CheckInDate, // "yyyy-MM-dd"
    int     Days,
    decimal Advance
);

public record PayRequest(
    string  BookingId,
    string  HoldId,
    string  Method,      // "cash" | "credit" | "check"
    string  Detail,
    decimal Amount
);

public record AddRoomRequest   (string RoomNumber, string Style, decimal Price, bool IsSmoking, string BranchId);
public record AddServiceRequest(string BookingId, string Type, string Description, decimal Cost);
