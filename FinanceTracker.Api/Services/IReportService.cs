using FinanceTracker.Api.DTOS.ReportDtos;

namespace FinanceTracker.Api.Services
{
    public interface IReportService
    {
        Task<MonthlyReportDto> GetMonthlyReportAsync(int userId, int year, int month);

    }
}
