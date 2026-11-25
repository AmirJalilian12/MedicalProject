using Application.DTOs.Orders;
using Application.Features.Orders.Commands.Create;
using Application.Features.Orders.Commands.Verify;
using Application.Features.Orders.Queries.GetDeliveryMethodById;
using Application.Features.Orders.Queries.GetDeliveryMethods;
using Application.Features.Orders.Queries.GetOrderByIdForUser;
using Application.Features.Orders.Queries.GetOrdersForUser;
using Domain.Entities.OrderEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Common;

namespace Web.Controllers
{
    [Authorize]
    public class OrderController : BaseApiController
    {
        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand request , CancellationToken cancellationToken) 
        {
            return Ok(await Mediator.Send(request,cancellationToken));
        }
        [HttpGet("GetOrdersForUser")]
        public async Task<ActionResult<List<OrderDto>>> GetOrdersForUser(CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new GetOrdersForUserQuery() ,cancellationToken));
        }
        [HttpGet("GetOrderByIdForUser/{id:int}")]
        public async Task<ActionResult<OrderDto>> GetOrderByIdForUser([FromRoute]int id , CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new GetOrderByIdForUserQuery(id),cancellationToken));
        }
        [HttpGet("GetDeliveryMethods")]
        [AllowAnonymous]
        public async Task<ActionResult<List<DeliveryMethod>>> GetDeliveryMethods(CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new GetDeliveryMethodsQuery() , cancellationToken));
        }
        [HttpGet("GetDeliveryMethodById/{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult>GetDeliveryMethodById(int id ,CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new GetDeliveryMethodByIdQuery(id), cancellationToken));
        }
        [HttpGet("ReturnToStore")]
        [AllowAnonymous]
        public IActionResult ReturnToStore([FromServices] IConfiguration configuration) =>
            Redirect(configuration["Order:StoreHome"] ?? "/");

        [HttpGet("Verify")]
        [AllowAnonymous]
        public async Task<IActionResult> Verify(string authority , string status , CancellationToken cancellationToken)
        {
            return Redirect(await Mediator.Send(new VerifyCommand(authority,status),cancellationToken));
        }
    }
}
