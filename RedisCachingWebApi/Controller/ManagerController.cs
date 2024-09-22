using MediatR;
using Microsoft.AspNetCore.Mvc;
using RedisCachingWebApi.Application.Handlers;

namespace RedisCachingWebApi.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ManagerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ManagerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST: api/manager
        [HttpPost]
        public async Task<ActionResult<SaveManagerHandler.Response>> AddManager([FromBody] SaveManagerHandler.Command command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}