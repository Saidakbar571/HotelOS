// Domain/Enums/RoomStatus.cs
namespace HotelOS.Domain.Enums;

public enum RoomStatus
{
    Available,      // Bo'sh, bron qilish mumkin
    Reserved,       // 10 daqiqa hold — to'lov kutilmoqda
    Occupied,       // Mehmon ichida
    BeingServiced,  // Tozalanmoqda (check-out dan keyin)
    NotAvailable    // Ta'mirlash yoki boshqa sabab
}
