public interface IBlogRepository {
    IEnumerable<Blog> GetAllBlogs();
    Blog GetBlogById(int id);
    void AddBlog(Blog blog);
    void UpdateBlog(Blog blog);
    void DeleteBlog(int id);
    public IEnumerable<Blog> GetAllBlogsByAuthor(string authorId);
}