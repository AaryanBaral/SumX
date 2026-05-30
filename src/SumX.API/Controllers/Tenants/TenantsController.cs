using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SumX.API.Models.Tenant;
using SumX.Application.Tenants.Commands.CreateTenant;
using SumX.Application.Tenants.Commands.DeleteTenant;
using SumX.Application.Tenants.Commands.UpdateTenant;
using SumX.Application.Tenants.Queries.GetAllTenants;
using SumX.Application.Tenants.Queries.GetTenantById;
using SumX.Domain.Constants;
using SumX.Domain.Entities.Master;

namespace SumX.API.Controllers.Tenants
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/tenants")]
    [Authorize(Roles = Roles.SuperAdmin)]
    public sealed class TenantsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TenantsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create(CreateTenantRequest request)
        {
            var command = new CreateTenantCommand(
                request.Name,
                request.Email,
                request.TenantId,
                request.DatabaseConnectionString,
                request.AdminPassword);

            var id = await _mediator.Send(command);
            return Ok(id);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Guid>> Update(Guid id, UpdateTenantRequest request)
        {
            var command = new UpdateTenantCommand(
                id,
                request.Name,
                request.Email,
                request.DatabaseConnectionString);

            var updatedId = await _mediator.Send(command);
            return Ok(updatedId);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Guid>> Delete(Guid id)
        {
            var command = new DeleteTenantCommand(id);
            var deletedId = await _mediator.Send(command);
            return Ok(deletedId);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Tenant>> GetById(Guid id)
        {
            var query = new GetTenantByIdQuery(id);
            var tenant = await _mediator.Send(query);
            return Ok(tenant);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tenant>>> GetAll()
        {
            var query = new GetAllTenantsQuery();
            var tenants = await _mediator.Send(query);
            return Ok(tenants);
        }
    }
}
