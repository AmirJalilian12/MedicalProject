using Application.DTOs.Account;
using Application.Features.Account.Commands.CreateAddress;
using Application.Features.Account.Commands.Register;
using Application.Features.Account.Queries.GetAddresses;
using Application.Features.Account.Queries.LoginUser;
using Application.Features.Account.Queries.MedicalProfile.UserMedicalProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Common;

namespace Web.Controllers
{
    public class AccountController : BaseApiController
    {
        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginUserQuery request , CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(request, cancellationToken));
        }
        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterCommand request , CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(request,cancellationToken));
        }
        [Authorize]
        [HttpPost("CreateAddress")]
        public async Task<ActionResult<AddressDto>> CreateAddress([FromBody] CreateAddressCommand request , CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(request,cancellationToken));
        }
        [Authorize]
        [HttpGet("GetAddresses")]
        public async Task<ActionResult<IEnumerable<AddressDto>>> GetAddresses(CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new GetAddressesQuery() , cancellationToken));
        }
        [Authorize]
        [HttpGet("GetMedProfile")]
        public async Task<ActionResult<MedProfileDto>> GetMedProfileForUser(CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new GetMedicalProfileForUserQuery(),cancellationToken);
            return Ok(result);
        }
    }
}
