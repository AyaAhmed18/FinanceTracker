using FinanceTracker.Api.DTOS.ReportDtos;
using FinanceTracker.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Api.Controllers;

public class ReportsController : ApiControllerBase
{
    private readonly IReportService _reports;
    public ReportsController(IReportService reports) => _reports = reports;

    [HttpGet("monthly")]
    public async Task<ActionResult<MonthlyReportDto>> Monthly(int year, int month)
    {
        if (year < 2000 || year > 2100 || month < 1 || month > 12)
            return BadRequest(new { message = "Invalid year or month." });

        return Ok(await _reports.GetMonthlyReportAsync(UserId, year, month));
    }
}