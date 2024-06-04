using DataBase.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserService.AuthorizationModel;

namespace UserService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RestrictedController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public RestrictedController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        [Route("AdminEndPoint")]
        [Authorize(Roles = "Администратор")]
        public IActionResult AdminEndPoint()
        {
            var currentUser = GetCurrentUser();
            return Ok($"Привет, ты {currentUser.Role}");
        }

        [HttpGet]
        [Route("UserEndPoint")]
        [Authorize(Roles = "Администратор, User")]
        public IActionResult UserEndPoint()
        {
            var currentUser = GetCurrentUser();
            return Ok($"Привет, ты {currentUser.Role}");
        }

        [HttpGet]
        [Route("AllUsers")]
        public IActionResult GetAllUsers()
        {
            var allUsers = _userRepository.GetAllUsers();
            return Ok(allUsers);
        }

        [HttpDelete]
        [Route("DeleteUser")]
        [Authorize(Roles = "Администратор")]
        public IActionResult DeleteUser(string name)
        {
            _userRepository.DeleteUser(name);
            return Ok();
        }

        private UserModel GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaims = identity.Claims;
                return new UserModel
                {
                    Username = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value,
                    Role = (UserRole)Enum.Parse(typeof(UserRole), userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value)
                };
            }

            return null;
        }
    }
}
