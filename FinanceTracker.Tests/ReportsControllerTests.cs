using FinanceTracker.Api.Controllers;
using FinanceTracker.Api.DTOS.ReportDtos;
using FinanceTracker.Api.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace FinanceTracker.Tests;

public class ReportsControllerTests
{
    private static ReportsController CreateController(Mock<IReportService> service, int userId = 1)
    {
        var controller = new ReportsController(service.Object);
        var identity = new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }, "test");
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
        return controller;
    }

    [Fact]
    public async Task Monthly_InvalidMonth_ReturnsBadRequest()
    {
        var service = new Mock<IReportService>();

        var result = await CreateController(service).Monthly(2026, 13);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
        service.Verify(s => s.GetMonthlyReportAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Monthly_ValidRequest_CallsServiceWithCurrentUser()
    {
        var service = new Mock<IReportService>();
        service.Setup(s => s.GetMonthlyReportAsync(5, 2026, 10))
               .ReturnsAsync(new MonthlyReportDto { Year = 2026, Month = 10 });

        var result = await CreateController(service, userId: 5).Monthly(2026, 10);

        result.Result.Should().BeOfType<OkObjectResult>();
        service.Verify(s => s.GetMonthlyReportAsync(5, 2026, 10), Times.Once);
    }
}