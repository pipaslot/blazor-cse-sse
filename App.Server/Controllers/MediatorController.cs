using System.Threading;
using System.Threading.Tasks;
using Core.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace App.Server.Controllers
{
    [Route("api/mediator")]
    [ApiController]
    public class MediatorController : ControllerBase
    {
        private readonly CommandQueryContractExecutor _executor;

        public MediatorController(IMediator mediator)
        {
            _executor = new CommandQueryContractExecutor(mediator);
        }

        [HttpPost("query")]
        public async Task<ActionResult> MediatorQuery([FromBody]CommandQueryContract contract, CancellationToken cancellationToken)
        {
            var result = await _executor.ExecuteQuery(contract, cancellationToken);
            return new JsonResult(result);
        }
        
        [HttpPost("command")]
        public async Task<ActionResult> MediatorCommand([FromBody]CommandQueryContract contract, CancellationToken cancellationToken)
        {
            var result = await _executor.ExecuteCommand(contract, cancellationToken);
            return new JsonResult(result);
        }
    }
}
