using CityGuide.Models;
using CityGuide.Repositories;
using CityGuide.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace CityGuide.Controllers
{
    public class UserController : ApiController
    {
        private readonly UserService _service;
        private readonly TokenService _jwtService;
        public UserController()
        {
            string filePath =
                System.Web.Hosting.HostingEnvironment.MapPath(
                    "~/App_Data/users.json");
            var repository = new UserRepository(filePath);
            _service = new UserService(repository);
            _jwtService = new TokenService();
        }

        [HttpPost]
        public IHttpActionResult Register([FromBody] User user)
        {
            if (user == null)
                return BadRequest("User data is required.");
            int result = _service.Register(user);
            switch (result)
            {
                case -1:
                    return BadRequest("Username and password are required.");
                case -2:
                    return BadRequest("Username already exists.");
                case -3:
                    return BadRequest("Email already exists.");
                case 0:
                    return BadRequest("Invalid user data.");
                default:
                    return Ok(result);
            }
        }

        [HttpPost]
        public IHttpActionResult Login([FromBody] User user)
        {
            if (user == null)
                return BadRequest("User data is required.");
            int result = _service.Login(user.Username, user.Password);
            if (result == 0)
                return Unauthorized();

            var token = _jwtService.GenerateToken(result);

            return Ok(new
            {
                Token = token,
                UserId = result
            });
        }

        [HttpGet]
        public IHttpActionResult GetUsernameById(int userId)
        {
            if (userId <= 0)
                return BadRequest("Invalid user ID.");
            var username = _service.GetUsernameById(userId);
            if (username == null)
                return NotFound();
            return Ok(username);
        }


    }
}