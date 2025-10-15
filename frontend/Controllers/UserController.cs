using frontend.Dto;
using frontend.Service;
using Microsoft.AspNetCore.Mvc;

namespace frontend.Controllers;

[Route("User")]
public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly HttpClientService _httpClientService;

    public UserController(ILogger<UserController> logger, HttpClientService httpClientService)
    {
        _logger = logger;
        _httpClientService = httpClientService;
    }

    [HttpGet("Index")]
    public async Task<IActionResult> Index()
    {
        var users = await _httpClientService.GetAsync<List<UserDto>>("User");
        return View(users);
    }

    [HttpGet("Edit")]
    public async Task<IActionResult> Edit(Guid Id)
    {
        var user = await _httpClientService.GetAsync<UserDto>($"User/{Id}");
        return View(user);
    }

    [HttpPost("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid Id, [FromForm] UserDto user)
    {
        await _httpClientService.PutAsync<UserDto, UserDto>($"User/{Id}", user);
        return RedirectToAction("Index");
    }

    [HttpGet("Create")]
    public IActionResult CreateUser()
    {
        var user = new UserDto();
        user.Id = Guid.NewGuid();
        return View("Create", user);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> CreateUser([FromForm] UserDto user)
    {
        await _httpClientService.PostAsync<dynamic, UserDto>("User", user);

        return RedirectToAction("Index");
    }

    [HttpDelete("Delete/{Id}")]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
    {
        await _httpClientService.DeleteAsync<dynamic>($"User/{id}");

        return Ok();
    }
}
