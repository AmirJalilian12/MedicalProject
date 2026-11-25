using Application.DTOs.Account;
using Domain.Entities.IdentityEntities.Medical;
using Mapster;

namespace Application.Common.Mapping.Configurations
{
    public class MedProfileMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<UserMedical, MedProfileDto>()
                .Map(
                    dest => dest.NationalCode,
                    src => src.NationalCode
                )
                .Map(
                    dest => dest.BloodGroup,
                    src => src.BloodGroup
                );
        }
    }
}