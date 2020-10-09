using System;
using System.Collections.Generic;
using BallanceRecordApi.Contracts.V1;
using BallanceRecordApi.Domain;
using Microsoft.AspNetCore.Mvc;

namespace BallanceRecordApi.Controllers.V1
{
    public class PostsController: Controller
    {
        private List<Post> _posts;

        public PostsController()
        {
            _posts = new List<Post>();
            for (var i = 0; i < 5; i++)
            {
                _posts.Add(new Post { Id = Guid.NewGuid().ToString() });
            }
        }

        [HttpGet(ApiRoutes.Posts.GetAll)]
        public IActionResult GetAll()
        {
            return Ok(_posts);
        }
    }
}
