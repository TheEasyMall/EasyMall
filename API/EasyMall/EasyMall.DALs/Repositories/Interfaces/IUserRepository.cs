using EasyMall.DALs.Data;
using EasyMall.DALs.Entities;
using Maynghien.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyMall.DALs.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User, ApplicationDbContext, ApplicationUser>
    {
    }
}
