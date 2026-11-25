using Domain.Entities.BaseEntities;
using Domain.Entities.IdentityEntities;

namespace Domain.Entities.IdentityEntities.Medical
{
    public class UserMedical : BaseAuditableEntity
    {
        public string UserId { get; set; }

        public User User { get; set; }

        public string NationalCode { get; set; }

        public string BloodGroup { get; set; }
    }
}