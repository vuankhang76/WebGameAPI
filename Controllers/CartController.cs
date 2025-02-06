using GameAccountStore.Data;
using GameAccountStore.DTOs;
using GameAccountStore.Helpers;
using GameAccountStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly GameAccountStoreContext _context;

    public CartController(GameAccountStoreContext context)
    {
        _context = context;
    }

    // GET: api/Cart
    [HttpGet]
    public async Task<ActionResult<ServiceResponse<Cart>>> GetCart()
    {
        var response = new ServiceResponse<Cart>();
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.GameAccount)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            response.Data = cart;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        return response;
    }

    // POST: api/Cart/items
    [HttpPost("items")]
    public async Task<ActionResult<ServiceResponse<CartItem>>> AddToCart(AddToCartDto request)
    {
        var response = new ServiceResponse<CartItem>();
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var gameAccount = await _context.GameAccounts.FindAsync(request.GameAccountId);
            if (gameAccount == null)
            {
                response.Success = false;
                response.Message = "Game account not found.";
                return NotFound(response);
            }

            if (gameAccount.Status != "Available")
            {
                response.Success = false;
                response.Message = "Game account is not available.";
                return BadRequest(response);
            }

            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.GameAccountId == request.GameAccountId);

            if (existingItem != null)
            {
                response.Success = false;
                response.Message = "Item already in cart.";
                return BadRequest(response);
            }

            var cartItem = new CartItem
            {
                CartId = cart.Id,
                GameAccountId = request.GameAccountId,
                CreatedAt = DateTime.Now
            };

            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();

            response.Data = cartItem;
            response.Message = "Item added to cart successfully.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        return response;
    }

    // DELETE: api/Cart/items/5
    [HttpDelete("items/{id}")]
    public async Task<ActionResult<ServiceResponse<bool>>> RemoveFromCart(int id)
    {
        var response = new ServiceResponse<bool>();
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == id && ci.Cart.UserId == userId);

            if (cartItem == null)
            {
                response.Success = false;
                response.Message = "Cart item not found.";
                return NotFound(response);
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            response.Data = true;
            response.Message = "Item removed from cart successfully.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        return response;
    }

    // DELETE: api/Cart/clear
    [HttpDelete("clear")]
    public async Task<ActionResult<ServiceResponse<bool>>> ClearCart()
    {
        var response = new ServiceResponse<bool>();
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
            {
                _context.CartItems.RemoveRange(cart.CartItems);
                await _context.SaveChangesAsync();
            }

            response.Data = true;
            response.Message = "Cart cleared successfully.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        return response;
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ServiceResponse<bool>>> DeleteCart(int id)
    {
        var response = new ServiceResponse<bool>();
        try
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cart == null)
            {
                response.Success = false;
                response.Message = "Cart not found.";
                return NotFound(response);
            }

            // Remove all cart items first
            if (cart.CartItems != null && cart.CartItems.Any())
            {
                _context.CartItems.RemoveRange(cart.CartItems);
            }

            // Then remove the cart
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            response.Data = true;
            response.Message = $"Cart ID {id} and all its items have been deleted successfully.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        return response;
    }
}