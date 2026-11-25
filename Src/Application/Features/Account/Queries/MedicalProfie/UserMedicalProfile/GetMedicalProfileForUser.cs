
using Application.DTOs.Account;
using Application.DTOs.Contracts;
using Domain.Exceptions;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Account.Queries.MedicalProfile.UserMedicalProfile
{
    public class GetMedicalProfileForUserQuery : IRequest<MedProfileDto>
    {

    }

    public class GetMedicalProfileForUserQueryHandler
        : IRequestHandler<GetMedicalProfileForUserQuery, MedProfileDto>
    {
        private readonly ICurrentService _currentService;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public GetMedicalProfileForUserQueryHandler(
            ICurrentService currentService,
            IMapper mapper,
            UserManager<User> userManager)
        {
            _currentService = currentService;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<MedProfileDto> Handle(
            GetMedicalProfileForUserQuery request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.Users
                .AsNoTracking()
                .Include(x => x.UserMedical)
                .FirstOrDefaultAsync(
                    x => x.Id == _currentService.UserId,
                    cancellationToken
                );

            // کاربر وجود ندارد
            if (user == null)
                throw new NotFoundException();

            // پروفایل پزشکی هنوز ایجاد نشده است
            if (user.UserMedical == null)
                return new MedProfileDto();

            // پروفایل پزشکی وجود دارد
            return _mapper.Map<MedProfileDto>(user.UserMedical);
        }
    }
}
