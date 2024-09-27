using oblig1.Data;
using Microsoft.EntityFrameworkCore; 

public class BlogRepository : IBlogRepository
{
    private readonly ApplicationDbContext _context;

    public BlogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void AddBlog(Blog blog)
    {
        _context.Blogs.Add(blog);
        _context.SaveChanges();
    }

    public void DeleteBlog(int id)
    {
        var blog = _context.Blogs
            .Include(b => b.Posts)
                .ThenInclude(p => p.Comments)
            .FirstOrDefault(b => b.Id == id);

        if (blog != null)
        {
            foreach (var post in blog.Posts)
            {
                _context.Comments.RemoveRange(post.Comments);
            }
            _context.Posts.RemoveRange(blog.Posts);
            _context.Blogs.Remove(blog);
            _context.SaveChanges();
        }
    }


    public IEnumerable<Blog> GetAllBlogs()
{
    return _context.Blogs
        .Include(b => b.Author) 
        .ToList();
}

    public Blog GetBlogById(int id)
    {
        return _context.Blogs
            .Include(b => b.Author)
            .Include(b => b.Posts)
                .ThenInclude(p => p.Comments)
                    .ThenInclude(c => c.User) 
            .FirstOrDefault(b => b.Id == id);
    }




    public void UpdateBlog(Blog blog)
    {
        _context.Blogs.Update(blog);
        _context.SaveChanges();
    }

    public IEnumerable<Blog> GetAllBlogsByAuthor(string authorId)
    {
        return _context.Blogs.Where(b => b.Author.Id == authorId).ToList();
    }

}
