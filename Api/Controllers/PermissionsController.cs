using Application.Logs.Querys;
using Application.Permissions.Commands;
using Application.Permissions.Querys;
using Domain.Permissions;
using Infrastructure.Audilog;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionsController : ControllerBase
    {
        private readonly IMediator _mediator;


        public PermissionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [TypeFilter(typeof(AuditableAttribute), Arguments = new object[] { "Request" })]
        public async Task<ActionResult> RequestPermission([FromBody] RequestPermissionCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPut("{id}")]
        [TypeFilter(typeof(AuditableAttribute), Arguments = new object[] { "Modify" })]
        public async Task<ActionResult> ModifyPermission(int id, [FromBody] ModifyPermissionCommand command)
        {
            if (command.Id != id) return BadRequest();
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpGet]
        [TypeFilter(typeof(AuditableAttribute), Arguments = new object[] { "Get" })]
        public async Task<ActionResult<List<Permission>>> GetPermissions()
        {
            var result = await _mediator.Send(new GetPermissionsQuery());
            return Ok(result);
        }


        [HttpPatch]
        public async Task<ActionResult<List<LogDto>>> GetLog()
        {
            var result = await _mediator.Send(new GetLogsQuery());
            return Ok(result);
        }

    }
}
