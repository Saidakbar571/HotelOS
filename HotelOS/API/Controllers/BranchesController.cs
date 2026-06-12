// API/Controllers/BranchesController.cs
// R10: Ko'p filial
using HotelOS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class BranchesController : ControllerBase
{
    private readonly IBranchService _branches;
    public BranchesController(IBranchService branches) => _branches = branches;

    /// <summary>Barcha filiallar — GET /api/branches</summary>
    [HttpGet]
    public IActionResult GetAll() => Ok(new { ok = true, branches = _branches.GetAll() });
}
