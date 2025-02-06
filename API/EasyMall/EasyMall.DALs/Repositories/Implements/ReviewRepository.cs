using EasyMall.DALs.Data;
using EasyMall.DALs.Entities;
using EasyMall.DALs.Repositories.Interfaces;
using Maynghien.Infrastructure.Repository;

namespace EasyMall.DALs.Repositories.Implements
{
    public class ReviewRepository : GenericRepository<Review, ApplicationDbContext, ApplicationUser>, IReviewRepository
    {
        public ReviewRepository(ApplicationDbContext unitOfWork) : base(unitOfWork)
        {
        }
    }
}
