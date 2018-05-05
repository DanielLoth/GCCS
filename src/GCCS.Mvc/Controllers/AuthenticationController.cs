using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GCCS.Mvc.Controllers
{
    public class AuthenticationController : Controller
    {
        public const string LoginRoute = "/authentication/login";

        private readonly IConfiguration configuration;

        public AuthenticationController(IConfiguration config)
        {
            configuration = config;
        }

        [Authorize]
        [HttpGet("/protected")]
        public string Protected()
        {
            return "Protected";
        }

        [AllowAnonymous]
        [HttpPost(LoginRoute)]
        public IActionResult CreateTokenFromBody([FromBody]LoginModel login) => CreateToken(login);

        private IActionResult CreateToken(LoginModel login)
        {
            var user = Authenticate(login);

            if (user != null)
            {
                var tokenString = BuildToken(user);
                return Ok(new { token = tokenString });
            }

            return Unauthorized();
        }

        private string BuildToken(UserModel user)
        {

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.Name),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Birthdate, user.Birthdate.ToString("yyyy-MM-dd")),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(configuration["Jwt:Issuer"],
              configuration["Jwt:Issuer"],
              claims,
              expires: DateTime.UtcNow.AddYears(10),
              signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private UserModel Authenticate(LoginModel login)
        {
            return new UserModel {
                Name = "Daniel",
                Email = "daniel@example.com",
                Birthdate = new DateTime(1990, 01, 01)
            };
        }

        public class LoginModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        private class UserModel
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public DateTime Birthdate { get; set; }
        }
    }
}