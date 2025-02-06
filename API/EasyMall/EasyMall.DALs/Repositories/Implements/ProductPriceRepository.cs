using EasyMall.DALs.Data;
using EasyMall.DALs.Entities;
using EasyMall.DALs.Repositories.Interfaces;
using Maynghien.Infrastructure.Repository;

namespace EasyMall.DALs.Repositories.Implements
{
    public class ProductPriceRepository : GenericRepository<ProductPrice, ApplicationDbContext, ApplicationUser>, IProductPriceRepository
    {
        public ProductPriceRepository(ApplicationDbContext unitOfWork) : base(unitOfWork)
        {
        }
    }
}
