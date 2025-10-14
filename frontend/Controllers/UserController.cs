using frontend.Dto;
using frontend.Service;
using Microsoft.AspNetCore.Mvc;

namespace frontend.Controllers;

public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly HttpClientService _httpClientService;

    public UserController(ILogger<UserController> logger, HttpClientService httpClientService)
    {
        _logger = logger;
        _httpClientService = httpClientService;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _httpClientService.GetAsync<List<UserDto>>("User");
        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid Id)
    {
        var user = await _httpClientService.GetAsync<UserDto>($"User/{Id}");
        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid Id, [FromForm] UserDto user)
    {
        await _httpClientService.PutAsync<UserDto, UserDto>($"User/{Id}", user);
        return RedirectToAction("Index");
    }
}
