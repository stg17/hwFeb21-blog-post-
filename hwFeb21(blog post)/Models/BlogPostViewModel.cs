namespace hwFeb21_blog_post_.Models
{
    public class BlogPostViewModel
    {
        public BlogPost BlogPost { get; set; }
        public string CommenterName { get; set; }
    }
    
    public class AllBlogPostsViewModel
    {
        public List<BlogPost> BlogPosts { get; set; }
        public int pageNum { get; set; }
        public bool Older { get; set; }
        public bool Newer { get; set; }
    }
}
