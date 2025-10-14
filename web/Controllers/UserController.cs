using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web.Domain.Data;
using web.Domain.Entities;

namespace web.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _context.Users.ToListAsync();
        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUsers(User user)
    {
        _context.Add(user);
        await _context.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created);
    }
}
