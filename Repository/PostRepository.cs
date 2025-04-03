using BTL_LTWNC.Data;
using BTL_LTWNC.Models;

namespace BTL_LTWNC.Repository
{
    public class PostRepository : IPostRepository
    {
        private readonly ApplicationDbContext _context;

        public PostRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddPost(PostModel post)
        {
            throw new NotImplementedException();
        }
    }
}
