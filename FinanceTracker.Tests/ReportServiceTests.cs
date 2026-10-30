using FinanceTracker.Api.Entities;
using FinanceTracker.Api.Services;
using FluentAssertions;

namespace FinanceTracker.Tests;

public class ReportServiceTests
{
    private static (Api.Data.AppDbContext db, int userId, Category salary, Category food) Seed()
    {
        var db = TestDb.Create();
        var user = new User { FullName = "Aya", Email = "aya@test.com", PasswordHash = "x" };
        db.Users.Add(user);
        db.SaveChanges();

        var salary = new Category { Name = "Salary", Type = TransactionType.Income, UserId = user.Id };
        var food = new Category { Name = "Food", Type = TransactionType.Expense, UserId = user.Id };
        db.Categories.AddRange(salary, food);
        db.SaveChanges();

        return (db, user.Id, salary, food);
    }

    private static Transaction Tx(int userId, Category c, decimal amount, DateTime date) => new()
    {
        UserId = userId,
        CategoryId = c.Id,
        Type = c.Type,
        Amount = amount,
        Date = date
    };

    [Fact]
    public async Task Monthly_ComputesIncomeExpensesAndBalance()
    {
        var (db, userId, salary, food) = Seed();
        db.Transactions.AddRange(
            Tx(userId, salary, 5000, new DateTime(2026, 10, 1)),
            Tx(userId, food, 200, new DateTime(2026, 10, 5)),
            Tx(userId, food, 300, new DateTime(2026, 10, 20)));
        db.SaveChanges();

        var report = await new ReportService(db).GetMonthlyReportAsync(userId, 2026, 10);

        report.TotalIncome.Should().Be(5000);
        report.TotalExpenses.Should().Be(500);
        report.Balance.Should().Be(4500);
    }

    [Fact]
    public async Task Monthly_GroupsByCategory()
    {
        var (db, userId, salary, food) = Seed();
        db.Transactions.AddRange(
            Tx(userId, food, 200, new DateTime(2026, 10, 5)),
            Tx(userId, food, 300, new DateTime(2026, 10, 20)),
            Tx(userId, salary, 5000, new DateTime(2026, 10, 1)));
        db.SaveChanges();

        var report = await new ReportService(db).GetMonthlyReportAsync(userId, 2026, 10);

        report.ByCategory.Should().HaveCount(2);
        var foodRow = report.ByCategory.Single(x => x.CategoryName == "Food");
        foodRow.Total.Should().Be(500);
        foodRow.Count.Should().Be(2);
    }

    [Fact]
    public async Task Monthly_IgnoresOtherMonths()
    {
        var (db, userId, _, food) = Seed();
        db.Transactions.AddRange(
            Tx(userId, food, 100, new DateTime(2026, 9, 30)),   // الشهر اللي قبله
            Tx(userId, food, 200, new DateTime(2026, 10, 15)),
            Tx(userId, food, 400, new DateTime(2026, 11, 1)));  // الشهر اللي بعده
        db.SaveChanges();

        var report = await new ReportService(db).GetMonthlyReportAsync(userId, 2026, 10);

        report.TotalExpenses.Should().Be(200);
    }

    [Fact]
    public async Task Monthly_IgnoresOtherUsers()
    {
        var (db, userId, _, food) = Seed();
        var other = new User { FullName = "Other", Email = "o@test.com", PasswordHash = "x" };
        db.Users.Add(other);
        db.SaveChanges();
        var otherFood = new Category { Name = "Food", Type = TransactionType.Expense, UserId = other.Id };
        db.Categories.Add(otherFood);
        db.SaveChanges();

        db.Transactions.AddRange(
            Tx(userId, food, 100, new DateTime(2026, 10, 5)),
            Tx(other.Id, otherFood, 9999, new DateTime(2026, 10, 5)));
        db.SaveChanges();

        var report = await new ReportService(db).GetMonthlyReportAsync(userId, 2026, 10);

        report.TotalExpenses.Should().Be(100);
    }

    [Fact]
    public async Task Monthly_EmptyMonth_ReturnsZeros()
    {
        var (db, userId, _, _) = Seed();

        var report = await new ReportService(db).GetMonthlyReportAsync(userId, 2026, 10);

        report.TotalIncome.Should().Be(0);
        report.TotalExpenses.Should().Be(0);
        report.Balance.Should().Be(0);
        report.ByCategory.Should().BeEmpty();
    }
}