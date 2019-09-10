using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.APi.Data;
using DatingApp.APi.DTOs;
using DatingApp.APi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.APi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
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

            return Ok(new {message = "Your request has been submitted"});
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(UserForLogInDTO userForLogInDTO)
        {
            var userFromRepo = await _repo.Login(userForLogInDTO.username.ToLower(), userForLogInDTO.password);

            if(userFromRepo == null)
            return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.userName),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new {token = tokenHandler.WriteToken(token), username = userForLogInDTO.username}); 
            

        }
    }
}