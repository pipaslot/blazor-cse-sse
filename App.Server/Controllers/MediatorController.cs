using System.Threading;
using System.Threading.Tasks;
using App.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace App.Server.Controllers
{
    [Route("api/mediator")]
    [ApiController]
    public class MediatorController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MediatorController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("request")]
        public async Task<ActionResult> MediatorRequest([FromBody]RequestNotificationContract commandQuery, CancellationToken cancellationToken)
        {
            var query = commandQuery.GetObject();
            var result = await _mediator.Send(query, cancellationToken);
            return new JsonResult(result);
        }
        
        [HttpPost("notification")]
        public async Task<ActionResult> MediatorNotification([FromBody]RequestNotificationContract commandQuery, CancellationToken cancellationToken)
        {
            var query = commandQuery.GetObject();
            await _mediator.Publish(query, cancellationToken);
            return Ok();
        }
    }
}
