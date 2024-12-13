using System.Security.Claims;
using FundRaising.Server.BLL.DTOs.Fundraiser;
using FundRaising.Server.BLL.Services.Fundraisers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FundRaising.Server.API.Controllers;

[ApiController]
[Route("/fundraisers")]
[Authorize]
public class FundraisersController: ControllerBase
{
    private readonly IFundraisersService _fundraisersService;

    public FundraisersController(
        IFundraisersService fundraisersService)
    {
        _fundraisersService = fundraisersService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserId();
        var fundraisers = await _fundraisersService
            .GetAllUserFundraisers(userId);
        
        return Ok(fundraisers);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> Get(Guid id)
    {
        var fundraiser = await _fundraisersService
            .GetFundraiser(id);
        
        return Ok(fundraiser);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateFundraiserDto fundraiserDto)
    {
        var userId = GetUserId();
        await _fundraisersService.AddFundraiser(userId, fundraiserDto);

        return Created();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] CreateFundraiserDto fundraiserDto)
    {
        var userId = GetUserId();
        await _fundraisersService.UpdateFundraiser(
            userId, id, fundraiserDto);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();
        await _fundraisersService.DeleteFundraiser(userId, id);
        
        return NoContent();
    }
    
    private Guid GetUserId()
    {
        var userId = HttpContext.User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        return new Guid(userId!);
    }
}