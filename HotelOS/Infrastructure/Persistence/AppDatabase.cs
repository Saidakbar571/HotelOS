// Infrastructure/Persistence/AppDatabase.cs
//
// In-memory "veritabani" — haqiqiy loyihada Entity Framework + SQL Server.
// Static klass: dastur ishlaganda bitta nusxa (Singleton pattern).
//
// Seed() — boshlang'ich ma'lumotlar bir marta yuklanadi.
using HotelOS.Domain.Entities;
using HotelOS.Domain.Enums;

namespace HotelOS.Infrastructure.Persistence;

public static class AppDatabase
{
    public static List<Guest>        Guests        { get; } = new();
    public static List<Receptionist> Receptionists { get; } = new();
    public static List<HouseKeeper>  HouseKeepers  { get; } = new();
    public static List<Branch>       Branches      { get; } = new();
    public static List<Room>         Rooms         { get; } = new();
    public static List<Booking>      Bookings      { get; } = new();
    public static List<Payment>      Payments      { get; } = new();
    public static List<Invoice>      Invoices      { get; } = new();
    public static List<CleaningTask> CleaningTasks { get; } = new();
    public static List<HotelService> HotelServices { get; } = new();

    static AppDatabase() => Seed();

    // -----------------------------------------------------------
    // Qidiruv yordamchilari — DRY: bir joyda, takrorlanmaydi
    // -----------------------------------------------------------
    public static Room?    FindRoom(string number)     => Rooms.FirstOrDefault(r => r.RoomNumber == number);
    public static Booking? FindBooking(string id)      => Bookings.FirstOrDefault(b => b.Id == id);
    public static Guest?   FindGuest(string accountId) => Guests.FirstOrDefault(g => g.Account.Id == accountId);

    public static (Person? Person, string Role) FindUser(string accountId, string password)
    {
        var guest = Guests.FirstOrDefault(g =>
            g.Account.Id == accountId && g.Account.Password == password &&
            g.Account.Status == AccountStatus.Active);
        if (guest != null) return (guest, "guest");

        var rec = Receptionists.FirstOrDefault(r =>
            r.Account.Id == accountId && r.Account.Password == password &&
            r.Account.Status == AccountStatus.Active);
        if (rec != null) return (rec, "receptionist");

        var hk = HouseKeepers.FirstOrDefault(h =>
            h.Account.Id == accountId && h.Account.Password == password &&
            h.Account.Status == AccountStatus.Active);
        if (hk != null) return (hk, "housekeeper");

        return (null, "");
    }

    // -----------------------------------------------------------
    // Boshlang'ich ma'lumotlar
    // -----------------------------------------------------------
    private static void Seed()
    {
        var downtown = new Branch
        {
            Name    = "GrandStay Downtown",
            Address = new Address { Street = "1 Amir Temur", City = "Toshkent", Country = "O'zbekiston" }
        };
        var airport = new Branch
        {
            Name    = "GrandStay Airport",
            Address = new Address { Street = "Aeroport yo'li 5", City = "Toshkent", Country = "O'zbekiston" }
        };

        var priceByStyle = new Dictionary<RoomStyle, decimal>
        {
            { RoomStyle.Standard,      100m },
            { RoomStyle.Deluxe,        180m },
            { RoomStyle.FamilySuite,   250m },
            { RoomStyle.BusinessSuite, 320m }
        };

        var roomConfigs = new[]
        {
            ("101", RoomStyle.Standard,      RoomStatus.Available),
            ("102", RoomStyle.Standard,      RoomStatus.Occupied),
            ("103", RoomStyle.Deluxe,        RoomStatus.Available),
            ("104", RoomStyle.Deluxe,        RoomStatus.BeingServiced),
            ("201", RoomStyle.FamilySuite,   RoomStatus.Available),
            ("202", RoomStyle.FamilySuite,   RoomStatus.Available),
            ("301", RoomStyle.BusinessSuite, RoomStatus.Available),
            ("302", RoomStyle.BusinessSuite, RoomStatus.Available),
        };

        foreach (var (number, style, status) in roomConfigs)
        {
            var room = new Room
            {
                RoomNumber    = number,
                Style         = style,
                Status        = status,
                PricePerNight = priceByStyle[style],
                IsSmoking     = number.EndsWith("2"),
                BranchId      = downtown.Id,
                Keys          = { new RoomKey { Barcode = $"KEY-{number}" } }
            };
            downtown.Rooms.Add(room);
            Rooms.Add(room);
        }

        foreach (var i in Enumerable.Range(1, 2))
        {
            var room = new Room
            {
                RoomNumber    = $"A10{i}",
                Style         = RoomStyle.Standard,
                PricePerNight = 100m,
                BranchId      = airport.Id
            };
            airport.Rooms.Add(room);
            Rooms.Add(room);
        }

        Branches.AddRange(new[] { downtown, airport });

        Receptionists.Add(new Receptionist
        {
            Name    = "Aziza Karimova",
            Email   = "aziza@grandstay.uz",
            Phone   = "+998901234567",
            Account = new Account { Id = "REC001", Password = "admin123" }
        });

        HouseKeepers.Add(new HouseKeeper
        {
            Name    = "Nodira Yusupova",
            Email   = "nodira@grandstay.uz",
            Phone   = "+998907654321",
            Account = new Account { Id = "HK001", Password = "hk123" }
        });

        Guests.Add(new Guest
        {
            Name    = "Jasur Toshmatov",
            Email   = "jasur@email.com",
            Phone   = "+998901112233",
            Account = new Account { Id = "GST001", Password = "guest123" }
        });

        CleaningTasks.Add(new CleaningTask
        {
            RoomNumber      = "104",
            Description     = "Standart tozalash",
            DurationMinutes = 45,
            HousekeeperId   = "HK001",
            HousekeeperName = "Nodira Yusupova"
        });
    }
}
