using Application.DTOs.Account;
using Application.Interfaces;
using Domain.Enums;
using Domain.Exceptions;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Account.Commands.Register
{
    public class RegisterCommand : IRequest<UserDto>, IMapFrom<User>
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
    }
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, UserDto>
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        public RegisterCommandHandler(IMapper mapper, UserManager<User> userManager, ITokenService tokenService)
        {
            _mapper = mapper;
            _userManager = userManager;
            _tokenService = tokenService;
        }
        public async Task<UserDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var checkUser = await _userManager.Users.AnyAsync(x=>x.PhoneNumber == request.PhoneNumber,cancellationToken);
            if (checkUser) throw new BadRequestException("شماره وارد شده تکراری میباشد");
            var user = _mapper.Map<User>(request);
            user.UserName = request.PhoneNumber;
            var result = await _userManager.CreateAsync(user,request.Password);
            if (!result.Succeeded) throw new BadRequestException(result.Errors.FirstOrDefault().Description);
            var roleResult = await _userManager.AddToRoleAsync(user , RoleType.User.ToString());
            if (!roleResult.Succeeded) throw new BadRequestException(roleResult.Errors.FirstOrDefault().Description);
            var mapUser = _mapper.Map<UserDto>(user);
            mapUser.Token = await _tokenService.CreateToken(user);
            return mapUser;
        }
    }
}
