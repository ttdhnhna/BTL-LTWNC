using BTL_LTWNC.Models;

namespace BTL_LTWNC.Repository
{
    public interface IPostRepository
    {
        void AddPost(Post post);
        List<PostvsVehicle> GetAllPostsWithVehicle();
    }
}
