using Backend.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/dashboard")]
public sealed class DashboardController(IDashboardRepository repo) : ControllerBase
{
    [HttpGet("summary")]
    public async Task<IActionResult> Summary(CancellationToken cancellationToken)
    {
        var res = await repo.GetSummaryAsync(cancellationToken);
        return Ok(res);
    }

    [HttpGet("daily-sales")]
    public async Task<IActionResult> DailySales([FromQuery] int days = 14, CancellationToken cancellationToken = default)
    {
        var res = await repo.GetDailySalesAsync(days, cancellationToken);
        return Ok(res);
    }

    [HttpGet("monthly-trend")]
    public async Task<IActionResult> MonthlyTrend([FromQuery] int months = 12, CancellationToken cancellationToken = default)
    {
        var res = await repo.GetMonthlyTrendAsync(months, cancellationToken);
        return Ok(res);
    }
}

