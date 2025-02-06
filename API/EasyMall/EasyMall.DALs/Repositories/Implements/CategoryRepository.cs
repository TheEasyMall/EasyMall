using EasyMall.DALs.Data;
using EasyMall.DALs.Entities;
using EasyMall.DALs.Repositories.Interfaces;
using Maynghien.Infrastructure.Repository;

namespace EasyMall.DALs.Repositories.Implements
{
    public class CategoryRepository : GenericRepository<Category, ApplicationDbContext, ApplicationUser>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext unitOfWork) : base(unitOfWork)
        {
        }

        public void AddProductToCategory(Category category, List<Product> products)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    category.CreatedOn = DateTime.UtcNow;
                    _context.Categories.Add(category);

                    foreach (var product in products)
                    {
                        product.CreatedOn = DateTime.UtcNow;
                        product.CategoryId = category.Id;
                    }

                    _context.Products.AddRange(products);
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message + " " + ex.StackTrace);
                }
            }
        }

        public void UpdateProductOnCategory(Category category, List<Product> updateProduct, List<Product> createProduct)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Categories.Update(category);
                    _context.UpdateRange(updateProduct);
                    _context.Products.AddRange(createProduct);
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message + " " + ex.StackTrace);
                }
            }
        }
    }
}
