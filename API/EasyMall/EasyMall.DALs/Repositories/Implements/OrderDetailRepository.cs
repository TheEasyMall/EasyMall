using EasyMall.DALs.Data;
using EasyMall.DALs.Entities;
using EasyMall.DALs.Repositories.Interfaces;
using Maynghien.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyMall.DALs.Repositories.Implements
{
    public class OrderDetailRepository : GenericRepository<OrderDetail, ApplicationDbContext, ApplicationUser>, IOrderDetailRepository
    {
        public OrderDetailRepository(ApplicationDbContext unitOfWork) : base(unitOfWork)
        {
        }
    }
}
