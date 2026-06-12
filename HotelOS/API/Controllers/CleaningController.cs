// API/Controllers/CleaningController.cs
using HotelOS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CleaningController : ControllerBase
{
    private readonly ICleaningService _cleaning;
    public CleaningController(ICleaningService cleaning) => _cleaning = cleaning;

    /// <summary>Barcha tozalash vazifalari — GET /api/cleaning</summary>
    [HttpGet]
    public IActionResult GetAll()
    {
        var tasks = _cleaning.GetAll().Select(t => new
        {
            t.Id,
            t.RoomNumber,
            t.Description,
            createdAt       = t.CreatedAt.ToString("HH:mm"),
            t.DurationMinutes,
            t.IsCompleted,
            t.HousekeeperName
        });
        return Ok(new { ok = true, tasks });
    }

    /// <summary>Vazifani tugallash — PUT /api/cleaning/{id}/complete</summary>
    [HttpPut("{id}/complete")]
    public IActionResult Complete(string id)
    {
        var (ok, msg) = _cleaning.Complete(id);
        return ok
            ? Ok(new { ok, message = msg })
            : NotFound(new { ok, message = msg });
    }
}
