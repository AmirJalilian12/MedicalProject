using Application.DTOs.Account;
using Application.DTOs.Contracts;
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

namespace Application.Features.Account.Queries.GetAddresses
{
    public class GetAddressesQuery : IRequest<IEnumerable<AddressDto>>
    {
    }
    public class GetAddressesQueryHandler : IRequestHandler<GetAddressesQuery, IEnumerable<AddressDto>>
    {
        private readonly ICurrentService _currentService;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        public GetAddressesQueryHandler(ICurrentService currentService, IMapper mapper, UserManager<User> userManager)
        {
            _currentService = currentService;
            _mapper = mapper;
            _userManager = userManager;
        }
        public async Task<IEnumerable<AddressDto>> Handle(GetAddressesQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.Include(x=>x.Addresses) 
                .FirstOrDefaultAsync(x=>x.Id == _currentService.UserId ,cancellationToken);
            if (user == null)
                throw new NotFoundException(); 
            return _mapper.Map<IEnumerable<AddressDto>>(user.Addresses);
        }
    }
}
