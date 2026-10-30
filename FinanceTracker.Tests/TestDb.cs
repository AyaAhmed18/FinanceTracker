using FinanceTracker.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Tests;

public static class TestDb
{
    // كل اختبار ياخد قاعدة بيانات جديدة ومنفصلة
    public static AppDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }
}