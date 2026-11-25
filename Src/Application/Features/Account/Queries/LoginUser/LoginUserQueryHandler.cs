using Application.DTOs.Account;
using Application.DTOs.Contracts;
using Application.Interfaces;
using Domain.Exceptions;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Account.Queries.LoginUser
{
    public class LoginUserQuery : IRequest<UserDto>
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }
    public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery , UserDto>
    {
        private readonly IMapper _mapper;
        private readonly SignInManager<User> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        public LoginUserQueryHandler(IMapper mapper, SignInManager<User> signInManager, IUnitOfWork unitOfWork, ITokenService tokenService)
        {
            _mapper = mapper;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }
        public async Task<UserDto> Handle(LoginUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Context.Set<User>()
                .FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber, cancellationToken);
            if (user == null) throw new BadRequestException("کاربری با این مشخصات بافت نشد");
            var result = await _signInManager.CheckPasswordSignInAsync(user,request.Password,false);
            if (!result.Succeeded) throw new BadRequestException("نام کاربری یا رمز عبور اشتباه میباشد");
            var mapUser = _mapper.Map<UserDto>(user);
            mapUser.Token = await _tokenService.CreateToken(user);
            return mapUser; 
        }
    }
}
