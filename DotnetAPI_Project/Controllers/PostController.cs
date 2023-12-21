using DotnetAPI_Project.Data;
using DotnetAPI_Project.Dtos;
using DotnetAPI_Project.Helpers;
using DotnetAPI_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI_Project.Controllers
{
    [Authorize]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dapper;

        public PostController(IConfiguration configuration)
        {
            _dapper = new DataContextDapper(configuration);
        }

        [HttpGet("Posts")]
        public IEnumerable<Post> GetPosts()
        {
            string sql = $@"SELECT [PostId]
                                  ,[UserId]
                                  ,[PostTitle]
                                  ,[PostContent]
                                  ,[PostCreated]
                                  ,[PostUpdated]
                          FROM [TutorialAppSchema].[Posts]";

            return _dapper.LoadData<Post>(sql);
        }

        [HttpGet("PostSingle/{postId}")]
        public Post GetSinglePost(int postId)
        {
            string sql = $@"SELECT [PostId]
                                  ,[UserId]
                                  ,[PostTitle]
                                  ,[PostContent]
                                  ,[PostCreated]
                                  ,[PostUpdated]
                    FROM [TutorialAppSchema].[Posts]
                    WHERE PostId = '{postId}'
            ";

            return _dapper.LoadDataSingle<Post>(sql);
        }

        [HttpGet("PostsByUser/{userId}")]
        public IEnumerable<Post> GetPostsByUserId(int userId)
        {
            string sql = $@"SELECT [PostId]
                                  ,[UserId]
                                  ,[PostTitle]
                                  ,[PostContent]
                                  ,[PostCreated]
                                  ,[PostUpdated]
                    FROM [TutorialAppSchema].[Posts]
                    WHERE UserId = '{userId}'
            ";

            return _dapper.LoadData<Post>(sql);
        }

        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyPosts(int userId)
        {
            string sql = $@"SELECT [PostId]
                                  ,[UserId]
                                  ,[PostTitle]
                                  ,[PostContent]
                                  ,[PostCreated]
                                  ,[PostUpdated]
                    FROM [TutorialAppSchema].[Posts]
                    WHERE UserId = '{this.User.FindFirst("userId")?.Value}'
            ";

            return _dapper.LoadData<Post>(sql);
        }

        [HttpGet("PostBySeach/{searchParam}")]
        public IEnumerable<Post> PostBySeach(string searchParam)
        {
            string sql = $@"SELECT [PostId]
                                  ,[UserId]
                                  ,[PostTitle]
                                  ,[PostContent]
                                  ,[PostCreated]
                                  ,[PostUpdated]
                    FROM [TutorialAppSchema].[Posts]
                    WHERE PostTitle LIKE '%{searchParam}%',
                    OR    PostContent LIKE '%{searchParam}%'
            ";

            return _dapper.LoadData<Post>(sql);
        }

        [HttpPost("Post")]
        public IActionResult AddPost(PostToAddDto postToAdd)
        {
            string sql = 
                $@"INSERT INTO [TutorialAppSchema].[Posts] (       
                        [UserId]
                        ,[PostTitle]
                        ,[PostContent]
                        ,[PostCreated]
                        ,[PostUpdated]) 
                values ('{this.User.FindFirst("userId")?.Value}',
                        '{StringHelper.EscapeQuoteString(postToAdd.PostTitle)}',
                        '{StringHelper.EscapeQuoteString(postToAdd.PostContent)}',
                        GETDATE(),
                        GETDATE())";

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to add new post");
        }

        [HttpPut("Post")]
        public IActionResult EditPost(PostToEditDto postToEdit)
        {
            string sql =
                $@"UPDATE [TutorialAppSchema].Posts
                   SET [PostTitle] = '{StringHelper.EscapeQuoteString(postToEdit.PostTitle)}',
	                   [PostContent] = '{StringHelper.EscapeQuoteString(postToEdit.PostContent)}',
	                   [PostUpdated] = GETDATE()
                   WHERE PostId = '{postToEdit.PostId}'
                   AND   UserId = '{this.User.FindFirst("userId")?.Value}'
                ";

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to edit post");
        }

        [HttpDelete("Post/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string sql = $"delete from [TutorialAppSchema].Posts where PostId = '{postId}'";

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to delete post");
        }
    }
}
