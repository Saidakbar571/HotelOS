// API/Controllers/RoomsController.cs
using HotelOS.Application.DTOs;
using HotelOS.Application.Interfaces;
using HotelOS.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace HotelOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class RoomsController : ControllerBase
{
    private readonly IRoomService _rooms;
    public RoomsController(IRoomService rooms) => _rooms = rooms;

    /// <summary>Barcha xonalar — GET /api/rooms</summary>
    [HttpGet]
    public IActionResult GetAll()
    {
        var rooms = _rooms.GetAll().Select(r => new
        {
            r.RoomNumber,
            style         = r.Style.ToString(),
            status        = r.Status.ToString(),
            pricePerNight = r.PricePerNight,
            r.IsSmoking,
            r.BranchId,
            available     = r.IsAvailable()
        });
        return Ok(new { ok = true, rooms });
    }

    /// <summary>Bo'sh xona qidirish — GET /api/rooms/search?style=Deluxe&checkIn=2025-06-01&days=3</summary>
    [HttpGet("search")]
    public IActionResult Search(
        [FromQuery] string style   = "Standard",
        [FromQuery] string checkIn = "",
        [FromQuery] int    days    = 1)
    {
        if (!Enum.TryParse<RoomStyle>(style, ignoreCase: true, out var roomStyle))
            return BadRequest(new { ok = false, message = $"Noto'g'ri xona turi: {style}" });

        DateTime.TryParse(checkIn, out var date);

        var rooms = _rooms.Search(roomStyle, date, days).Select(r => new
        {
            r.RoomNumber,
            style         = r.Style.ToString(),
            pricePerNight = r.PricePerNight,
            r.IsSmoking
        });
        return Ok(new { ok = true, rooms });
    }

    /// <summary>Yangi xona — POST /api/rooms</summary>
    [HttpPost]
    public IActionResult Add([FromBody] AddRoomRequest req)
    {
        if (!Enum.TryParse<RoomStyle>(req.Style, ignoreCase: true, out var style))
            return BadRequest(new { ok = false, message = $"Noto'g'ri tur: {req.Style}" });

        var (ok, msg) = _rooms.Add(req.RoomNumber, style, req.Price, req.IsSmoking, req.BranchId);
        return ok ? Ok(new { ok, message = msg }) : BadRequest(new { ok, message = msg });
    }

    /// <summary>Holat yangilash — PUT /api/rooms/{number}/status?status=Available</summary>
    [HttpPut("{number}/status")]
    public IActionResult UpdateStatus(string number, [FromQuery] string status)
    {
        if (!Enum.TryParse<RoomStatus>(status, ignoreCase: true, out var roomStatus))
            return BadRequest(new { ok = false, message = $"Noto'g'ri holat: {status}" });

        var (ok, msg) = _rooms.UpdateStatus(number, roomStatus);
        return ok ? Ok(new { ok, message = msg }) : BadRequest(new { ok, message = msg });
    }
}
