// API/Controllers/HotelServicesController.cs
using HotelOS.Application.DTOs;
using HotelOS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class HotelServicesController : ControllerBase
{
    private readonly IHotelServiceManager _manager;
    public HotelServicesController(IHotelServiceManager manager) => _manager = manager;

    /// <summary>Bron xizmatlari — GET /api/hotelservices?bookingId=RES-XXX</summary>
    [HttpGet]
    public IActionResult Get([FromQuery] string bookingId)
    {
        var services = _manager.GetByBooking(bookingId).Select(s => new
        {
            s.Id, s.Type, s.Description, s.Cost,
            createdAt = s.CreatedAt.ToString("HH:mm")
        });
        return Ok(new { ok = true, services });
    }

    /// <summary>Xizmat qo'shish — POST /api/hotelservices</summary>
    [HttpPost]
    public IActionResult Add([FromBody] AddServiceRequest req)
    {
        var (ok, msg) = _manager.Add(req.BookingId, req.Type, req.Description, req.Cost);
        return ok
            ? Ok(new { ok, message = msg })
            : BadRequest(new { ok, message = msg });
    }
}
