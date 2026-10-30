using FinanceTracker.Api.Data;
using FinanceTracker.Api.DTOS;
using FinanceTracker.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Api.Controllers;

public class TransactionsController : ApiControllerBase
{
    private readonly AppDbContext _db;
    public TransactionsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<TransactionDto>>> GetAll(
        DateTime? from, DateTime? to, int? categoryId, TransactionType? type)
    {
        var q = _db.Transactions.Where(t => t.UserId == UserId);

        if (from.HasValue) q = q.Where(t => t.Date >= from.Value);
        if (to.HasValue) q = q.Where(t => t.Date <= to.Value);
        if (categoryId.HasValue) q = q.Where(t => t.CategoryId == categoryId.Value);
        if (type.HasValue) q = q.Where(t => t.Type == type.Value);

        var items = await q
            .OrderByDescending(t => t.Date)
            .Select(t => new TransactionDto
            {
                Id = t.Id,
                Amount = t.Amount,
                Type = t.Type,
                Date = t.Date,
                Note = t.Note,
                CategoryId = t.CategoryId,
                CategoryName = t.Category.Name
            })
            .ToListAsync();

        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TransactionDto>> Get(int id)
    {
        var t = await _db.Transactions.Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == UserId);
        return t is null ? NotFound() : Ok(ToDto(t));
    }

    [HttpPost]
    public async Task<ActionResult<TransactionDto>> Create(TransactionCreateDto dto)
    {
        var category = await _db.Categories
            .FirstOrDefaultAsync(c => c.Id == dto.CategoryId && c.UserId == UserId);
        if (category is null)
            return BadRequest(new { message = "Category not found." });

        var t = new Transaction
        {
            Amount = dto.Amount,
            Date = dto.Date,
            Note = dto.Note?.Trim(),
            CategoryId = category.Id,
            Type = category.Type,
            UserId = UserId
        };

        _db.Transactions.Add(t);
        await _db.SaveChangesAsync();
        t.Category = category;

        return CreatedAtAction(nameof(Get), new { id = t.Id }, ToDto(t));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, TransactionCreateDto dto)
    {
        var t = await _db.Transactions.FirstOrDefaultAsync(x => x.Id == id && x.UserId == UserId);
        if (t is null) return NotFound();

        var category = await _db.Categories
            .FirstOrDefaultAsync(c => c.Id == dto.CategoryId && c.UserId == UserId);
        if (category is null)
            return BadRequest(new { message = "Category not found." });

        t.Amount = dto.Amount;
        t.Date = dto.Date;
        t.Note = dto.Note?.Trim();
        t.CategoryId = category.Id;
        t.Type = category.Type;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var t = await _db.Transactions.FirstOrDefaultAsync(x => x.Id == id && x.UserId == UserId);
        if (t is null) return NotFound();

        _db.Transactions.Remove(t);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static TransactionDto ToDto(Transaction t) => new()
    {
        Id = t.Id,
        Amount = t.Amount,
        Type = t.Type,
        Date = t.Date,
        Note = t.Note,
        CategoryId = t.CategoryId,
        CategoryName = t.Category.Name
    };
}