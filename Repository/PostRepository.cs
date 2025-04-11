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

        public void AddPost(Post post)
        {
            _context.Add(post);
            _context.SaveChanges();
        }

        public List<PostvsVehicle> GetAllPostsWithVehicle()
        {
            var postvsvehicle = (from post in _context.tbl_Post
                                 join vehicle in _context.tbl_Vehicle
                                 on post.vehicleID equals vehicle.PK_iVehicleID
                                 select new PostvsVehicle
                                 {
                                     Post = post,
                                     Vehicle = vehicle
                                 }).ToList();

            return postvsvehicle;
        }
    }
}
