using GameAccountStore.Data;
using GameAccountStore.Helpers;
using GameAccountStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly GameAccountStoreContext _context;

    public CategoryController(GameAccountStoreContext context)
    {
        _context = context;
    }

    // GET: api/Category
    [HttpGet]
    public async Task<ActionResult<ServiceResponse<List<Category>>>> GetCategories()
    {
        var response = new ServiceResponse<List<Category>>();
        try
        {
            response.Data = await _context.Categories.ToListAsync();
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        return response;
    }

    // GET: api/Category/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResponse<Category>>> GetCategory(int id)
    {
        var response = new ServiceResponse<Category>();
        try
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                response.Success = false;
                response.Message = "Category not found.";
                return NotFound(response);
            }
            response.Data = category;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        return response;
    }

    // POST: api/Category
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ServiceResponse<Category>>> CreateCategory(Category category)
    {
        var response = new ServiceResponse<Category>();
        try
        {
            category.CreatedAt = DateTime.Now;
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            response.Data = category;
            response.Message = "Category created successfully.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        return response;
    }

    // PUT: api/Category/5
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<ServiceResponse<Category>>> UpdateCategory(int id, Category category)
    {
        var response = new ServiceResponse<Category>();
        try
        {
            if (id != category.Id)
            {
                response.Success = false;
                response.Message = "Invalid Id.";
                return BadRequest(response);
            }

            category.UpdatedAt = DateTime.Now;
            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            response.Data = category;
            response.Message = "Category updated successfully.";
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CategoryExists(id))
            {
                response.Success = false;
                response.Message = "Category not found.";
                return NotFound(response);
            }
            throw;
        }
        return response;
    }

    // DELETE: api/Category/5
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ServiceResponse<bool>>> DeleteCategory(int id)
    {
        var response = new ServiceResponse<bool>();
        try
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                response.Success = false;
                response.Message = "Category not found.";
                return NotFound(response);
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            response.Data = true;
            response.Message = "Category deleted successfully.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        return response;
    }

    private bool CategoryExists(int id)
    {
        return _context.Categories.Any(e => e.Id == id);
    }
}