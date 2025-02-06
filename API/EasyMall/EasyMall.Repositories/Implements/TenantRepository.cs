using EasyMall.DALs.Data;
using EasyMall.DALs.Entities;
using EasyMall.DALs.Repositories.Interfaces;
using Maynghien.Infrastructure.Repository;

namespace EasyMall.DALs.Repositories.Implements
{
    public class TenantRepository : GenericRepository<Tenant, ApplicationDbContext, ApplicationUser>, ITenantRepository
    {
        public TenantRepository(ApplicationDbContext unitOfWork) : base(unitOfWork)
        {
        }
    }
}
