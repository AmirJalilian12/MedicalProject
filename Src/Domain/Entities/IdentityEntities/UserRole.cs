using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Domain.Entities.IdentityEntities
{
    public class UserRole : IdentityUserRole<string>
    {
        public UserRole(string roleId, string userId)
        {
            RoleId = roleId;
            UserId = userId;
        }

        public UserRole() { }

        public User User { get; set; }     // ← اضافه کنید
        public Role Role { get; set; }     // ← اضافه کنید
    }
}
