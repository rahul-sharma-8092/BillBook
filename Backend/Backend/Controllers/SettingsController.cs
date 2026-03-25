using Backend.Application.Settings;
using Backend.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/settings")]
public sealed class SettingsController(ISettingsRepository repo) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var res = await repo.GetAsync(cancellationToken);
        return Ok(res);
    }

    [HttpPut]
    public async Task<IActionResult> Upsert([FromBody] SettingUpsertDto dto, CancellationToken cancellationToken)
    {
        var res = await repo.UpsertAsync(new
        {
            dto.ShopName,
            dto.Address,
            dto.Phone,
            dto.Email,
            dto.BankName,
            dto.AccountName,
            dto.AccountNumber,
            dto.IFSC,
            dto.UPI,
            dto.Terms,
            dto.FooterMessage
        }, cancellationToken);
        return Ok(res);
    }
}

