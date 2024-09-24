using Oblig1.Data; 
using oblig1.Models;  
using System.Collections.Generic;
using System.Linq;

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
        var blog = _context.Blogs.Find(id);
        if (blog != null)
        {
            _context.Blogs.Remove(blog);
            _context.SaveChanges();
        }
    }

    public IEnumerable<Blog> GetAllBlogs()
    {
        return _context.Blogs.ToList();
    }

    public Blog GetBlogById(int id)
    {
        return _context.Blogs.Find(id);
    }

    public void UpdateBlog(Blog blog)
    {
        _context.Blogs.Update(blog);
        _context.SaveChanges();
    }
}
