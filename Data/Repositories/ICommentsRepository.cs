public interface ICommentsRepository
{
    void AddComment(Comment comment);
    IEnumerable<Comment> GetCommentsByPostId(int postId);
    void DeleteComment(int id);
}
