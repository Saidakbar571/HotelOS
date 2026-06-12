// API/Controllers/AuthController.cs
using HotelOS.Application.DTOs;
using HotelOS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;

    /// <summary>Login — POST /api/auth/login</summary>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest req)
    {
        var (ok, role, id, name) = _auth.Login(req.AccountId, req.Password);
        return ok
            ? Ok(new { ok = true, role, accountId = id, name })
            : Unauthorized(new { ok = false, message = "Noto'g'ri login yoki parol." });
    }

    /// <summary>Ro'yxatdan o'tish — POST /api/auth/register</summary>
    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest req)
    {
        var (ok, msg, id) = _auth.Register(req.Name, req.Email, req.Phone, req.Password);
        return ok
            ? Ok(new { ok = true, message = msg, accountId = id })
            : BadRequest(new { ok = false, message = msg });
    }
}
