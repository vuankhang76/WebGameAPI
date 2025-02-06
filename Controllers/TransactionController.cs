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
public class TransactionController : ControllerBase
{
    private readonly GameAccountStoreContext _context;

    public TransactionController(GameAccountStoreContext context)
    {
        _context = context;
    }

    // GET: api/Transaction
    [HttpGet]
    public async Task<ActionResult<ServiceResponse<List<Transaction>>>> GetMyTransactions()
    {
        var response = new ServiceResponse<List<Transaction>>();
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            response.Data = await _context.Transactions
                .Include(t => t.GameAccount)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        return response;
    }

    // POST: api/Transaction/checkout
    [HttpPost("checkout")]
    public async Task<ActionResult<ServiceResponse<Transaction>>> Checkout()
    {
        var response = new ServiceResponse<Transaction>();
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.GameAccount)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart?.CartItems == null || !cart.CartItems.Any())
            {
                response.Success = false;
                response.Message = "Cart is empty.";
                return BadRequest(response);
            }

            var user = await _context.Users.FindAsync(userId);
            var totalAmount = cart.CartItems.Sum(ci => ci.GameAccount.Price);

            if (user.Balance < totalAmount)
            {
                response.Success = false;
                response.Message = "Insufficient balance.";
                return BadRequest(response);
            }

            // Create transactions for each game account
            foreach (var item in cart.CartItems)
            {
                var gameAccount = item.GameAccount;
                if (gameAccount.Status != "Available")
                {
                    response.Success = false;
                    response.Message = $"Game account {gameAccount.Title} is no longer available.";
                    return BadRequest(response);
                }

                var newTransaction = new Transaction
                {
                    UserId = userId,
                    GameAccountId = gameAccount.Id,
                    Amount = gameAccount.Price,
                    Status = "Completed",
                    CreatedAt = DateTime.Now
                };

                gameAccount.Status = "Sold";
                user.Balance -= gameAccount.Price;

                _context.Transactions.Add(newTransaction);
            }

            // Clear cart
            _context.CartItems.RemoveRange(cart.CartItems);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            response.Message = "Checkout completed successfully.";
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            response.Success = false;
            response.Message = ex.Message;
        }
        return response;
    }

    // GET: api/Transaction/all (Admin only)
    [Authorize(Roles = "Admin")]
    [HttpGet("all")]
    public async Task<ActionResult<ServiceResponse<List<Transaction>>>> GetAllTransactions()
    {
        var response = new ServiceResponse<List<Transaction>>();
        try
        {
            response.Data = await _context.Transactions
                .Include(t => t.User)
                .Include(t => t.GameAccount)
                .OrderByDescending(t => t.CreatedAt)
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
