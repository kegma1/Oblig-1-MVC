using oblig1.Data;
using oblig1.Models;
using System.Collections.Generic;
using System.Linq;
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
        var blog = _context.Blogs.Find(id);
        if (blog != null)
        {
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

}
