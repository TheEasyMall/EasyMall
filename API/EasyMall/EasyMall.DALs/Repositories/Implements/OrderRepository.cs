using EasyMall.DALs.Data;
using EasyMall.DALs.Entities;
using EasyMall.DALs.Repositories.Interfaces;
using Maynghien.Infrastructure.Repository;

namespace EasyMall.DALs.Repositories.Implements
{
    public class OrderRepository : GenericRepository<Order, ApplicationDbContext, ApplicationUser>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext unitOfWork) : base(unitOfWork)
        {
        }
    }
}
