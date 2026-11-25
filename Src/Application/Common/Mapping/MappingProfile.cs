using Application.DTOs.Account;
using Application.Features.Account.Commands.Register;
using Domain.Entities.IdentityEntities;
using Mapster;

namespace Application.Common.Mapping
{
    public class MappingProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // User <-> UserDto
            config.NewConfig<User, UserDto>().TwoWays();

            // RegisterCommand -> User  (UserName باید پر شود وگرنه Identity رد می‌کند)
            config.NewConfig<RegisterCommand, User>()
                  .Map(dest => dest.UserName, src => src.PhoneNumber)
                  .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
                  .Map(dest => dest.DisplayName, src => src.DisplayName);
            // Address <-> AddressDto
            config.NewConfig<Address, AddressDto>().TwoWays();

        }
    }
}
