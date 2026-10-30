using FinanceTracker.Api.Data;
using FinanceTracker.Api.DTOS;
using FinanceTracker.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Api.Controllers;

public class CategoriesController : ApiControllerBase
{
    private readonly AppDbContext _db;
    public CategoriesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetAll()
    {
        var items = await _db.Categories
            .Where(c => c.UserId == UserId)
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto { Id = c.Id, Name = c.Name, Type = c.Type })
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> Get(int id)
    {
        var c = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id && x.UserId == UserId);
        if (c is null) return NotFound();
        return Ok(new CategoryDto { Id = c.Id, Name = c.Name, Type = c.Type });
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(CategoryCreateDto dto)
    {
        var name = dto.Name.Trim();
        if (await _db.Categories.AnyAsync(c => c.UserId == UserId && c.Name == name && c.Type == dto.Type))
            return Conflict(new { message = "Category already exists." });

        var c = new Category { Name = name, Type = dto.Type, UserId = UserId };
        _db.Categories.Add(c);
        await _db.SaveChangesAsync();

        var result = new CategoryDto { Id = c.Id, Name = c.Name, Type = c.Type };
        return CreatedAtAction(nameof(Get), new { id = c.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CategoryCreateDto dto)
    {
        var c = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id && x.UserId == UserId);
        if (c is null) return NotFound();

        if (c.Type != dto.Type && await _db.Transactions.AnyAsync(t => t.CategoryId == id))
            return BadRequest(new { message = "Cannot change the type of a category that has transactions." });

        c.Name = dto.Name.Trim();
        c.Type = dto.Type;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var c = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id && x.UserId == UserId);
        if (c is null) return NotFound();

        if (await _db.Transactions.AnyAsync(t => t.CategoryId == id))
            return BadRequest(new { message = "Cannot delete a category that has transactions." });

        _db.Categories.Remove(c);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}