using Application.DTOs.Account;
using Application.DTOs.Contracts;
using Domain.Entities.IdentityEntities;
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

namespace Application.Features.Account.Commands.CreateAddress
{
    public class CreateAddressCommand : IRequest<AddressDto>
    {
        public bool IsMain { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullAddress { get; set; }
        public string Number { get; set; }
        public string PostalCode { get; set; }
    }
    public class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommand, AddressDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentService _currentService;
        private readonly UserManager<User> _userManager;
        public CreateAddressCommandHandler(ICurrentService currentService, IMapper mapper, IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _currentService = currentService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<AddressDto> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentService.UserId;
            var user = await _userManager.Users.Include(x => x.Addresses)
                .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
            if (user == null)
                throw new NotFoundException("کاربری با این مشخصات یافت نشد");
            if(request.IsMain && user.Addresses.Any())
                user.Addresses.ForEach(x=>x.IsMain = false);
            if (!user.Addresses.Any())
                request.IsMain = true;
            var entity = _mapper.Map<Address>(request);
            entity.UserId = userId;
            user.Addresses.Add(entity);
            var userResult = await _userManager.UpdateAsync(user);
            if (!userResult.Succeeded)
                throw new BadRequestException();
            return _mapper.Map<AddressDto>(userResult);
        }
    }
}
