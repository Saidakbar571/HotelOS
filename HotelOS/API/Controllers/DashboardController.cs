// API/Controllers/DashboardController.cs
using HotelOS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboard;
    public DashboardController(IDashboardService dashboard) => _dashboard = dashboard;

    /// <summary>Umumiy statistika — GET /api/dashboard</summary>
    [HttpGet]
    public IActionResult Get() => Ok(_dashboard.GetStats());
}
