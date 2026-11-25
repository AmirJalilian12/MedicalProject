using Domain.Entities.IdentityEntities;
using Domain.Entities.IdentityEntities.Medical;
using Microsoft.AspNetCore.Identity;

public class User : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
    public string NationalCode { get; set; } = string.Empty;
    public List<Address> Addresses { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
    public UserMedical UserMedical { get; set; }
}

