using oblig1.Data;
using Microsoft.EntityFrameworkCore;

public class CommentsRepository : ICommentsRepository
{
    private readonly ApplicationDbContext _context;

    public CommentsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void AddComment(Comment comment)
    {
        _context.Comments.Add(comment);
        _context.SaveChanges();
    }

    public IEnumerable<Comment> GetCommentsByBlogId(int blogId)
    {
        return _context.Comments
            .Include(c => c.User)
            .Where(c => c.BlogId == blogId)
            .ToList();
    }

    public IEnumerable<Comment> GetCommentsByPostId(int postId)
    {
        return _context.Comments
            .Include(c => c.User)
            .Where(c => c.PostId == postId)
            .ToList();
    }

    public void DeleteComment(int id)
    {
        var comment = _context.Comments.Find(id);
        if (comment != null)
        {
            _context.Comments.Remove(comment);
            _context.SaveChanges();
        }
    }
}
