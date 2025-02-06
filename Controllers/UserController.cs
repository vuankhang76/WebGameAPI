using GameAccountStore.Data;
using GameAccountStore.Helpers;
using GameAccountStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly GameAccountStoreContext _context;

    public UserController(GameAccountStoreContext context)
    {
        _context = context;
    }

    // GET: api/User/profile
    [HttpGet("profile")]
    public async Task<ActionResult<ServiceResponse<User>>> GetProfile()
    {
        var response = new ServiceResponse<User>();
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found.";
                return NotFound(response);
            }
            response.Data = user;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        return response;
    }

    // PUT: api/User/balance/add
    [HttpPut("balance/add")]
    public async Task<ActionResult<ServiceResponse<decimal>>> AddBalance([FromBody] decimal amount)
    {
        var response = new ServiceResponse<decimal>();
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found.";
                return NotFound(response);
            }

            user.Balance += amount;
            await _context.SaveChangesAsync();

            response.Data = user.Balance;
            response.Message = $"Added {amount:C} to balance successfully.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        return response;
    }

    // GET: api/User/all (Admin only)
    [Authorize(Roles = "Admin")]
    [HttpGet("all")]
    public async Task<ActionResult<ServiceResponse<List<User>>>> GetAllUsers()
    {
        var response = new ServiceResponse<List<User>>();
        try
        {
            response.Data = await _context.Users
                .OrderBy(u => u.Username)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        return response;
    }
}
