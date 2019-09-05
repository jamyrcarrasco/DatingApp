using System.Threading.Tasks;
using DatingApp.APi.Data;
using DatingApp.APi.DTOs;
using DatingApp.APi.Models;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.APi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        public AuthController(IAuthRepository repo)
        {
            _repo = repo;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register (UserForRegisterDTO userForRegisterDTO)
        {
            // validate request

            userForRegisterDTO.Username = userForRegisterDTO.Username.ToLower();

            if(userForRegisterDTO.Username == null || userForRegisterDTO.Username == "")
                return BadRequest("The username cannot be empty");

            if (await _repo.UserExists(userForRegisterDTO.Username))
                return BadRequest("Username is already taken");

            var userToCreate = new User
            {
                userName = userForRegisterDTO.Username
            };

            var createdUser = await _repo.Register(userToCreate, userForRegisterDTO.Password);

            return StatusCode(201);
        }
    }
}