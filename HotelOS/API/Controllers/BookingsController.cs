// API/Controllers/BookingsController.cs
//
// Bron oqimi — 2 qadam:
//   POST /api/bookings/hold → Qadam 1: band qilish
//   POST /api/bookings/pay  → Qadam 2: to'lov
//
// SOLID SRP  : Faqat HTTP — biznes mantiq BookingService da.
// SOLID DIP  : IBookingService, IHoldService orqali.
using HotelOS.Application.DTOs;
using HotelOS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class BookingsController : ControllerBase
{
    private readonly IBookingService _bookings;
    private readonly IHoldService    _holds;

    public BookingsController(IBookingService bookings, IHoldService holds)
    {
        _bookings = bookings;
        _holds    = holds;
    }

    /// <summary>Barcha bronlar — GET /api/bookings</summary>
    [HttpGet]
    public IActionResult GetAll()
        => Ok(new { ok = true, bookings = _bookings.GetAll() });

    /// <summary>Bitta bron — GET /api/bookings/{id}</summary>
    [HttpGet("{id}")]
    public IActionResult GetOne(string id)
    {
        var result = _bookings.GetOne(id);
        return result is null
            ? NotFound(new { ok = false, message = "Bron topilmadi." })
            : Ok(new { ok = true, booking = result });
    }

    /// <summary>
    /// Qadam 1: Band qilish — POST /api/bookings/hold
    /// Javob: bookingId + holdId → /pay ga yuboriladi
    /// </summary>
    [HttpPost("hold")]
    public async Task<IActionResult> Hold([FromBody] HoldRequest req)
    {
        if (!DateTime.TryParse(req.CheckInDate, out var checkIn))
            return BadRequest(new { ok = false, message = "Noto'g'ri sana (yyyy-MM-dd)." });

        var (ok, msg, bookingId, holdId) =
            await _bookings.HoldRoomAsync(req.GuestId, req.RoomNumber, checkIn, req.Days, req.Advance);

        return ok
            ? Ok(new
              {
                  ok = true, message = msg,
                  bookingId, holdId,
                  expiresAt = DateTime.Now.AddMinutes(10).ToString("HH:mm:ss")
              })
            : BadRequest(new { ok = false, message = msg });
    }

    /// <summary>
    /// Qadam 2: To'lov — POST /api/bookings/pay
    /// OK → Confirmed | TTL yoki FAIL → Abandoned
    /// </summary>
    [HttpPost("pay")]
    public async Task<IActionResult> Pay([FromBody] PayRequest req)
    {
        var (ok, msg) = await _bookings.ConfirmPaymentAsync(
            req.BookingId, req.HoldId, req.Method, req.Detail, req.Amount);

        return ok
            ? Ok(new { ok = true, message = msg })
            : BadRequest(new { ok = false, message = msg });
    }

    /// <summary>Bekor qilish (R5) — PUT /api/bookings/{id}/cancel</summary>
    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> Cancel(string id)
    {
        var (ok, msg) = await _bookings.CancelAsync(id);
        return ok ? Ok(new { ok, message = msg }) : BadRequest(new { ok, message = msg });
    }

    /// <summary>Check-in — PUT /api/bookings/{id}/checkin</summary>
    [HttpPut("{id}/checkin")]
    public async Task<IActionResult> CheckIn(string id)
    {
        var (ok, msg) = await _bookings.CheckInAsync(id);
        return ok ? Ok(new { ok, message = msg }) : BadRequest(new { ok, message = msg });
    }

    /// <summary>Check-out — PUT /api/bookings/{id}/checkout</summary>
    [HttpPut("{id}/checkout")]
    public async Task<IActionResult> CheckOut(string id)
    {
        var (ok, msg, total) = await _bookings.CheckOutAsync(id);
        return ok ? Ok(new { ok, message = msg, total }) : BadRequest(new { ok, message = msg });
    }

    /// <summary>Faol holdlar soni — GET /api/bookings/holds/count</summary>
    [HttpGet("holds/count")]
    public IActionResult HoldsCount()
        => Ok(new { ok = true, activeHolds = _holds.ActiveCount });
}
