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
    public interface ICategoryRepository : IGenericRepository<Category, ApplicationDbContext, ApplicationUser>
    {
        void AddProductToCategory(Category category, List<Product> products);
        void UpdateProductOnCategory(Category category, List<Product> updateProduct, List<Product> createProduct);
    }
}
