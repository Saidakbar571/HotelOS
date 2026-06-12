// Domain/Enums/OtherEnums.cs
namespace HotelOS.Domain.Enums;

public enum RoomStyle    { Standard, Deluxe, FamilySuite, BusinessSuite }
public enum UserRole     { Guest, Receptionist, HouseKeeper, Manager }
public enum AccountStatus{ Active, Closed, Blacklisted }
public enum PaymentStatus{ Pending, Completed, Failed, Refunded }
