using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyMall.DALs.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [ForeignKey("Tenant")]
        public Guid? TenantId { get; set; }
        public Tenant? Tenant { get; set; }
    }
}
