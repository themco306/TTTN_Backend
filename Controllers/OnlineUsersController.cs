using backend.Hubs;
using Microsoft.AspNetCore.Mvc;
namespace backend.Controllers
{
[Route("api/hubUser")]
[ApiController]
public class OnlineUsersController : ControllerBase
{
    private readonly UserTracker _userTracker;

    public OnlineUsersController(UserTracker userTracker)
    {
        _userTracker = userTracker;
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetOnlineUserCount()
    {
        var count = await _userTracker.GetOnlineUserCount();
        return Ok(count);
    }

    [HttpGet]
    public async Task<IActionResult> GetOnlineUsers()
    {
        var users = await _userTracker.GetOnlineUsers();
        return Ok(users);
    }
}
}