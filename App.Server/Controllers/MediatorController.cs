using System.Threading;
using System.Threading.Tasks;
using Core.Mediator;
using Core.Mediator.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace App.Server.Controllers
{
    [Route("api/mediator")]
    [ApiController]
    public class MediatorController : ControllerBase
    {
        private readonly RequestContractExecutor _executor;

        public MediatorController(IMediator mediator)
        {
            _executor = new RequestContractExecutor(mediator);
        }

        [HttpPost("request")]
        public async Task<ActionResult> MediatorQuery([FromBody]RequestContract contract, CancellationToken cancellationToken)
        {
            var result = await _executor.ExecuteQuery(contract, cancellationToken);
            return new JsonResult(result);
        }
    }
}
