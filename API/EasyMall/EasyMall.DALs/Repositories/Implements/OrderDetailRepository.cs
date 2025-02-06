using EasyMall.DALs.Data;
using EasyMall.DALs.Entities;
using EasyMall.DALs.Repositories.Interfaces;
using Maynghien.Infrastructure.Repository;

namespace EasyMall.DALs.Repositories.Implements
{
    public class OrderDetailRepository : GenericRepository<OrderDetail, ApplicationDbContext, ApplicationUser>, IOrderDetailRepository
    {
        public OrderDetailRepository(ApplicationDbContext unitOfWork) : base(unitOfWork)
        {
        }
    }
}
