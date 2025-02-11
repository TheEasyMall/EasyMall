using EasyMall.DALs.Data;
using EasyMall.DALs.Entities;
using EasyMall.DALs.Repositories.Interfaces;
using Maynghien.Infrastructure.Repository;

namespace EasyMall.DALs.Repositories.Implements
{
    public class VariantRepository : GenericRepository<Variant, ApplicationDbContext, ApplicationUser>, IVariantRepository
    {
        public VariantRepository(ApplicationDbContext unitOfWork) : base(unitOfWork)
        {
        }
    }
}
