using GameAccountStore.Data;
using GameAccountStore.DTOs;
using GameAccountStore.Helpers;
using GameAccountStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class GameAccountController : ControllerBase
{
    private readonly GameAccountStoreContext _context;

    public GameAccountController(GameAccountStoreContext context)
    {
        _context = context;
    }

    // GET: api/GameAccount
    [HttpGet]
    public async Task<ActionResult<ServiceResponse<List<GameAccountResponseDto>>>> GetGameAccounts()
    {
        var response = new ServiceResponse<List<GameAccountResponseDto>>();
        try
        {
            var gameAccounts = await _context.GameAccounts
                .Include(g => g.Category)
                .Include(g => g.GameAccountImages)
                .ToListAsync();

            response.Data = gameAccounts.Select(g => new GameAccountResponseDto
            {
                Id = g.Id,
                CategoryId = g.CategoryId,
                CategoryName = g.Category.Name,
                Title = g.Title,
                Description = g.Description,
                GameType = g.GameType,
                Price = g.Price,
                Status = g.Status,
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdatedAt,
                ImageUrls = g.GameAccountImages.Select(i => i.ImageUrl).ToList()
            }).ToList();
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        return response;
    }

    // GET: api/GameAccount/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResponse<GameAccountResponseDto>>> GetGameAccount(int id)
    {
        var response = new ServiceResponse<GameAccountResponseDto>();
        try
        {
            var gameAccount = await _context.GameAccounts
                .Include(g => g.Category)
                .Include(g => g.GameAccountImages)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (gameAccount == null)
            {
                response.Success = false;
                response.Message = "Game account not found.";
                return NotFound(response);
            }

            response.Data = new GameAccountResponseDto
            {
                Id = gameAccount.Id,
                CategoryId = gameAccount.CategoryId,
                CategoryName = gameAccount.Category.Name,
                Title = gameAccount.Title,
                Description = gameAccount.Description,
                GameType = gameAccount.GameType,
                Price = gameAccount.Price,
                Status = gameAccount.Status,
                CreatedAt = gameAccount.CreatedAt,
                UpdatedAt = gameAccount.UpdatedAt,
                ImageUrls = gameAccount.GameAccountImages.Select(i => i.ImageUrl).ToList()
            };
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        return response;
    }

    // POST: api/GameAccount
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ServiceResponse<GameAccountResponseDto>>> CreateGameAccount(CreateGameAccountDto request)
    {
        var response = new ServiceResponse<GameAccountResponseDto>();
        try
        {
            var category = await _context.Categories.FindAsync(request.CategoryId);
            if (category == null)
            {
                response.Success = false;
                response.Message = "Category not found.";
                return BadRequest(response);
            }

            var gameAccount = new GameAccount
            {
                CategoryId = request.CategoryId,
                Title = request.Title,
                Description = request.Description,
                GameType = request.GameType,
                Price = request.Price,
                Status = "Available",
                CreatedAt = DateTime.Now
            };

            _context.GameAccounts.Add(gameAccount);
            await _context.SaveChangesAsync();

            if (request.ImageUrls != null && request.ImageUrls.Any())
            {
                foreach (var imageUrl in request.ImageUrls)
                {
                    var gameAccountImage = new GameAccountImage
                    {
                        GameAccountId = gameAccount.Id,
                        ImageUrl = imageUrl,
                        CreatedAt = DateTime.Now
                    };
                    _context.GameAccountImages.Add(gameAccountImage);
                }
                await _context.SaveChangesAsync();
            }

            response.Data = new GameAccountResponseDto
            {
                Id = gameAccount.Id,
                CategoryId = gameAccount.CategoryId,
                CategoryName = category.Name,
                Title = gameAccount.Title,
                Description = gameAccount.Description,
                GameType = gameAccount.GameType,
                Price = gameAccount.Price,
                Status = gameAccount.Status,
                CreatedAt = gameAccount.CreatedAt,
                UpdatedAt = gameAccount.UpdatedAt,
                ImageUrls = request.ImageUrls ?? new List<string>()
            };
            response.Message = "Game account created successfully.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        return response;
    }

    // PUT: api/GameAccount/5
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<ServiceResponse<GameAccount>>> UpdateGameAccount(int id, GameAccount gameAccount)
    {
        var response = new ServiceResponse<GameAccount>();
        try
        {
            if (id != gameAccount.Id)
            {
                response.Success = false;
                response.Message = "Invalid Id.";
                return BadRequest(response);
            }

            gameAccount.UpdatedAt = DateTime.Now;
            _context.Entry(gameAccount).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            response.Data = gameAccount;
            response.Message = "Game account updated successfully.";
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!GameAccountExists(id))
            {
                response.Success = false;
                response.Message = "Game account not found.";
                return NotFound(response);
            }
            throw;
        }
        return response;
    }

    // DELETE: api/GameAccount/5
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ServiceResponse<bool>>> DeleteGameAccount(int id)
    {
        var response = new ServiceResponse<bool>();
        try
        {
            var gameAccount = await _context.GameAccounts.FindAsync(id);
            if (gameAccount == null)
            {
                response.Success = false;
                response.Message = "Game account not found.";
                return NotFound(response);
            }

            _context.GameAccounts.Remove(gameAccount);
            await _context.SaveChangesAsync();

            response.Data = true;
            response.Message = "Game account deleted successfully.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        return response;
    }

    // GET: api/GameAccount/category/{categoryId}
    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<ServiceResponse<List<GameAccount>>>> GetGameAccountsByCategory(int categoryId)
    {
        var response = new ServiceResponse<List<GameAccount>>();
        try
        {
            response.Data = await _context.GameAccounts
                .Include(g => g.Category)
                .Include(g => g.GameAccountImages)
                .Where(g => g.CategoryId == categoryId)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        return response;
    }

    private bool GameAccountExists(int id)
    {
        return _context.GameAccounts.Any(e => e.Id == id);
    }
}