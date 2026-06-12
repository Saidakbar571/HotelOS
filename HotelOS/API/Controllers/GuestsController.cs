// API/Controllers/GuestsController.cs
using HotelOS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class GuestsController : ControllerBase
{
    private readonly IGuestService _guests;
    public GuestsController(IGuestService guests) => _guests = guests;

    /// <summary>Barcha mehmonlar — GET /api/guests</summary>
    [HttpGet]
    public IActionResult GetAll() => Ok(new { ok = true, guests = _guests.GetAll() });
}
