using FinanceTracker.Api.Data;
using FinanceTracker.Api.DTOS.ReportDtos;
using FinanceTracker.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Api.Services;

public class ReportService : IReportService
{
    private readonly AppDbContext _db;
    public ReportService(AppDbContext db) => _db = db;

    public async Task<MonthlyReportDto> GetMonthlyReportAsync(int userId, int year, int month)
    {
        var start = new DateTime(year, month, 1);
        var end = start.AddMonths(1);

        var byCategory = await _db.Transactions
            .Where(t => t.UserId == userId && t.Date >= start && t.Date < end)
            .GroupBy(t => new { t.CategoryId, t.Category.Name, t.Type })
            .Select(g => new CategorySummaryDto
            {
                CategoryId = g.Key.CategoryId,
                CategoryName = g.Key.Name,
                Type = g.Key.Type,
                Total = g.Sum(x => x.Amount),
                Count = g.Count()
            })
            .OrderByDescending(x => x.Total)
            .ToListAsync();

        var income = byCategory.Where(x => x.Type == TransactionType.Income).Sum(x => x.Total);
        var expenses = byCategory.Where(x => x.Type == TransactionType.Expense).Sum(x => x.Total);

        return new MonthlyReportDto
        {
            Year = year,
            Month = month,
            TotalIncome = income,
            TotalExpenses = expenses,
            Balance = income -expenses,
            ByCategory = byCategory
        };
    }
}