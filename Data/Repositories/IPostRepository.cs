public interface IPostRepository
{
    IEnumerable<Post> GetAllPosts();
    Post GetPostById(int id);
    void AddPost(Post post);
    void UpdatePost(Post post);
}
