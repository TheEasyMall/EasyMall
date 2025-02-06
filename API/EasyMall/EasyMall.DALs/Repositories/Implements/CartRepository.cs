using EasyMall.DALs.Data;
using EasyMall.DALs.Entities;
using EasyMall.DALs.Repositories.Interfaces;
using Maynghien.Infrastructure.Repository;

namespace EasyMall.DALs.Repositories.Implements
{
    public class CartRepository : GenericRepository<Cart, ApplicationDbContext, ApplicationUser>, ICartRepository
    {
        public CartRepository(ApplicationDbContext unitOfWork) : base(unitOfWork)
        {
        }
    }
}
