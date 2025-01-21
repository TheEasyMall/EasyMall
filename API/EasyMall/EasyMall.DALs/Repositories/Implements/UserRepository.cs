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
    public class UserRepository : GenericRepository<User, ApplicationDbContext, ApplicationUser>, IUserRepository
    {
        public UserRepository(ApplicationDbContext unitOfWork) : base(unitOfWork)
        {
        }
    }
}
