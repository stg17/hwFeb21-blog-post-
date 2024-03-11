using System.Data.SqlClient;

namespace hwFeb21_blog_post_.Models
{
    public class BlogPostsDBManager
    {
        private readonly string _connectionString;
        public BlogPostsDBManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddComment(Comment comment)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO Comments
                                    VALUES (@text, @name, @id, @date)";
            command.Parameters.AddWithValue("@text", comment.Text);
            command.Parameters.AddWithValue("@name", comment.CommenterName);
            command.Parameters.AddWithValue("@id", comment.BlogPostId);
            command.Parameters.AddWithValue("@date", DateTime.Now);
            connection.Open();
            command.ExecuteNonQuery();
        }

        public int AddPost(BlogPost blogPost)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO BlogPosts
                                    VALUES (@text, @title, @date)
                                    SELECT SCOPE_IDENTITY()";
            command.Parameters.AddWithValue("@text", blogPost.BlogText);
            command.Parameters.AddWithValue("@title", blogPost.Title);
            command.Parameters.AddWithValue("@date", DateTime.Now);
            connection.Open();
            return (int)(decimal)command.ExecuteScalar();
        }

        public List<BlogPost> GetBlogPosts()
        {
            return GetBlogPosts(0, 0);
        }

        public BlogPost GetBlogPostById(int id)
        {
            return GetBlogPosts().FirstOrDefault(b => b.Id == id);
        }

        public List<BlogPost> GetBlogPosts(int from, int till)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            bool addComments = till == 0;
            if (!addComments)
            {

                command.CommandText += @" SELECT * FROM BlogPosts b
                                        ORDER BY b.DateSubmitted desc
                                        OFFSET @from ROWS
                                        FETCH NEXT @till ROWS ONLY";
                command.Parameters.AddWithValue("@from", from);
                command.Parameters.AddWithValue("@till", till);
            }
            else
            {
                command.CommandText += @"SELECT c.Text, c.CommenterName, c.datecommented, b.* FROM BlogPosts b
                                        LEFT JOIN Comments c
                                        ON c.BlogPostId = b.Id
                                        ORDER BY b.DateSubmitted desc";
            }
            
            connection.Open();
            List<BlogPost> blogPosts = new();
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var blogId = (int)reader["Id"];
                var post = blogPosts.FirstOrDefault(b => b.Id == blogId);
                if (post == null)
                {
                    post = new()
                    {
                        Id = blogId,
                        BlogText = (string)reader["BlogText"],
                        Title = (string)reader["Title"],
                        DateSubmitted = (DateTime)reader["DateSubmitted"]
                    };
                    post.ShortenedBlogText = post.BlogText.ShortenText();
                    blogPosts.Add(post);

                }
                if (addComments)
                {
                    if (reader.GetOrNull<string>("commenterName") != null)
                    {
                        post.Comments.Add(new()
                        {
                            BlogPostId = blogId,
                            CommenterName = (string)reader["CommenterName"],
                            Text = (string)reader["Text"],
                            DateCommented = (DateTime)reader["DateCommented"]
                        });
                    }
                }

            }

            return blogPosts;

        }
    }

    public class BlogPost
    {
        public int Id { get; set; }
        public string BlogText { get; set; }
        public string ShortenedBlogText { get; set; }
        public string Title { get; set; }
        public DateTime DateSubmitted { get; set; }
        public List<Comment> Comments { get; set; } = new();
    }

    public class Comment
    {
        public string CommenterName { get; set; }
        public string Text { get; set; }
        public int BlogPostId { get; set; }
        public DateTime DateCommented { get; set; }
    }

   public static class Extensions
    {
        public static T GetOrNull<T>(this SqlDataReader reader, string columnName)
        {
            var value = reader[columnName];
            if (value == DBNull.Value)
            {
                return default(T);
            }

            return (T)value;
        }

        public static string ShortenText(this string text)
        {
            if (text.Count() <= 188)
            {
                return text;
            }

            return text.Substring(0, 188) + "...";
        }

        public static string cookieName { get; set; }
    }
}
