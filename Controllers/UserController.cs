using GameAccountStore.Data;
using GameAccountStore.DTOs;
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

    // POST: api/User/add-balance (Admin only)
    [Authorize(Roles = "Admin")]
    [HttpPost("add-balance")]
    public async Task<ActionResult<ServiceResponse<User>>> AddBalanceToUser([FromBody] AddBalanceDto request)
    {
        var response = new ServiceResponse<User>();
        try
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found.";
                return NotFound(response);
            }

            user.Balance += request.Amount;
            await _context.SaveChangesAsync();

            response.Data = user;
            response.Message = $"Successfully added {request.Amount:C} to user's balance.";
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