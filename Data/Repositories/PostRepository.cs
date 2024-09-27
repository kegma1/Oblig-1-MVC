using oblig1.Data;
public class PostRepository : IPostRepository
{
    private readonly ApplicationDbContext _context;

    public PostRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void AddPost(Post post)
    {
        _context.Posts.Add(post);
        _context.SaveChanges();
    }

    public void DeletePost(int id)
    {
        var post = _context.Posts.Find(id);
        if (post != null)
        {
            _context.Posts.Remove(post);
            _context.SaveChanges();
        }
    }

    public IEnumerable<Post> GetAllPosts()
    {
        return _context.Posts.ToList();
    }

    public Post GetPostById(int id)
    {
        return _context.Posts.Find(id);
    }

    public void UpdatePost(Post post)
    {
        _context.Posts.Update(post);
        _context.SaveChanges();
    }
    public void AddComment(Comment comment)
    {
        _context.Comments.Add(comment);
        _context.SaveChanges();
    }
    public IEnumerable<Comment> GetCommentsByPostId(int postId)
    {
        return _context.Comments.Where(c => c.PostId == postId).ToList();
    }
}
