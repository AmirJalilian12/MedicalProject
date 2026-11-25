using Application.DTOs.Common;
using Domain.Entities.IdentityEntities.Medical;
using Mapster;

namespace Application.DTOs.Account
{
    public class MedProfileDto : BaseAuditableDtoResponse, IMapFrom<UserMedical>
    {
        public string? NationalCode { get; set; }

        public string? BloodGroup { get; set; }
    }
}