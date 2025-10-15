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

    [HttpGet]
    [Route("{Id}")]
    public async Task<IActionResult> GetUserById(Guid Id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == Id);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPut]
    [Route("{Id}")]
    public async Task<IActionResult> EditUserById(Guid Id, [FromBody] User userBody)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == Id);
        if (user == null)
            return NotFound();

        user.Name = userBody.Name;
        user.Email = userBody.Email;
        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return Ok(user);
    }

    [HttpDelete]
    [Route("{Id}")]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid Id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == Id);
        if (user == null) return NotFound();
        _context.Remove(user);
        await _context.SaveChangesAsync();
        return StatusCode(StatusCodes.Status204NoContent);
    }
}
