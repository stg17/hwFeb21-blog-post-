using hwFeb21_blog_post_.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Xml.Linq;

namespace hwFeb21_blog_post_.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=BlogPosts; Integrated Security=true;";

        public IActionResult Index(int page = 1)
        {
            var manager = new BlogPostsDBManager(_connectionString);
            var list = manager.GetBlogPosts(((page - 1) * 3) + (page > 1 ? 1 : 0), 3);
            var viewModel = new AllBlogPostsViewModel 
            { 
                BlogPosts = list,
                pageNum = page 
            };
            viewModel.Newer = page > 1;
            viewModel.Older = list.Count == 3; //work on soon
            return View(viewModel);
        }

        public IActionResult Admin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SubmitPost(BlogPost bp)
        {
            var manager = new BlogPostsDBManager(_connectionString);
            int id = manager.AddPost(bp);
            return Redirect($"/home/viewblog?id={id}");
        }

        public IActionResult ViewBlog(int id)
        {
            var name = Request.Cookies["commenter-name"];
            var manager = new BlogPostsDBManager(_connectionString);
            var post = manager.GetBlogPostById(id);
            if (post == null)
            {
                return Redirect("/home/index");
            }

            return View(new BlogPostViewModel { BlogPost = post, CommenterName = name});
        }

        [HttpPost]
        public IActionResult AddComment(Comment comment)
        {
            Response.Cookies.Append("commenter-name", comment.CommenterName);
            var manager = new BlogPostsDBManager(_connectionString);
            manager.AddComment(comment);
            return Redirect($"/home/viewBlog?id={comment.BlogPostId}");
        }

        public IActionResult MostRecent()
        {
            var manager = new BlogPostsDBManager(_connectionString);
            var blog = manager.GetBlogPosts(0, 1).FirstOrDefault();
            return Redirect($"/home/viewBlog?id={blog.Id}");
        }
    }
}