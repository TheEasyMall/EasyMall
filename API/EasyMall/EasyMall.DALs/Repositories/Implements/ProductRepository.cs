using EasyMall.DALs.Data;
using EasyMall.DALs.Entities;
using EasyMall.DALs.Repositories.Interfaces;
using Maynghien.Infrastructure.Repository;

namespace EasyMall.DALs.Repositories.Implements
{
    public class ProductRepository : GenericRepository<Product, ApplicationDbContext, ApplicationUser>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext unitOfWork) : base(unitOfWork)
        {
        }
    }
}
