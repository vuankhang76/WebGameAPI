using GameAccountStore.Data;
using GameAccountStore.DTOs;
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

    // GET: api/Category/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResponse<CategoryDto>>> GetCategory(int id)
    {
        var response = new ServiceResponse<CategoryDto>();
        try
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                response.Success = false;
                response.Message = "Category not found.";
                return NotFound(response);
            }

            response.Data = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };
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
    public async Task<ActionResult<ServiceResponse<CategoryDto>>> CreateCategory(CreateCategoryDto request)
    {
        var response = new ServiceResponse<CategoryDto>();
        try
        {
            var category = new Category
            {
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTime.Now
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            response.Data = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };
            response.Message = "Category created successfully.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        return response;
    }

    // PUT: api/Category/{id}
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<ServiceResponse<CategoryDto>>> UpdateCategory(int id, CreateCategoryDto request)
    {
        var response = new ServiceResponse<CategoryDto>();
        try
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                response.Success = false;
                response.Message = "Category not found.";
                return NotFound(response);
            }

            category.Name = request.Name;
            category.Description = request.Description;
            category.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            response.Data = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };
            response.Message = "Category updated successfully.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
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