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

        // Get Manager By Id
        [HttpGet("{managerId:int}")]
        public async Task<ActionResult<GetManagerByIdHandler.Response>> GetMangerByID([FromRoute] GetManagerByIdHandler.Query query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        // Get All Manager Data
        [HttpGet()]
        public async Task<ActionResult<GetAllManagerHandler.Response>> GetAllManger([FromRoute] GetAllManagerHandler.Query query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        // Delete Manger By Id
        [HttpDelete("{managerId:int}")]
        public async Task<ActionResult<DeleteManagerByIdHandler.Response>> DeleteMangerByID([FromRoute] DeleteManagerByIdHandler.Command command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}