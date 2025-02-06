using EasyMall.DALs.Data;
using EasyMall.DALs.Entities;
using Maynghien.Infrastructure.Repository;

namespace EasyMall.DALs.Repositories.Interfaces
{
    public interface ITenantRepository : IGenericRepository<Tenant, ApplicationDbContext, ApplicationUser>
    {
    }
}
