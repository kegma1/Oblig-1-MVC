public interface ICommentsRepository
{
    void AddComment(Comment comment);
    IEnumerable<Comment> GetCommentsByBlogId(int blogId);
    IEnumerable<Comment> GetCommentsByPostId(int postId);
    void DeleteComment(int id);
}
