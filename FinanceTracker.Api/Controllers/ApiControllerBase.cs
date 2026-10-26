using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinanceTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]

    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected int UserId =>
       int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
