public interface IPostRepository {
    IEnumerable<Post> GetAllPosts();
    Post GetPostById(int id);
    void AddPost(Post post);
    void UpdatePost(Post post);
    // New methods for handling comments on posts
    void AddComment(Comment comment);
    IEnumerable<Comment> GetCommentsByPostId(int postId);
}