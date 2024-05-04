using Microsoft.AspNetCore.Mvc;
using RestAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class AAuthTokenController
    {
        private readonly ILogger<AAuthTokenController> _logger;
        private readonly SuperheroesContext _superheroesContext;

        private StringBuilder userLogin { get; set; } = new StringBuilder();
        private StringBuilder userRole { get; set; } = new StringBuilder();

        public AAuthTokenController(ILogger<AAuthTokenController> logger, SuperheroesContext superheroesContext)
        {
            _logger = logger;
            _superheroesContext = superheroesContext;
        }

        //генерация токена, если введены верные данные (существующих пользователей)
        [HttpPost(Name = "Login")]
        public IActionResult GenAuthToken(String login, String password)
        {
            var user = _superheroesContext.AuthPersons.FirstOrDefault(u => u.Login == login && u.Password == password);

            if (user == null)
                return new BadRequestObjectResult("Неверное имя пользователя и/или пароль");

            var user_claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Login),
                                           new Claim(ClaimTypes.Role, user.Role) };
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    claims: user_claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(1)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            // формируем ответ
            var response = new
            {
                access_token = encodedJwt,
                username = user.Login
            };

            userLogin.AppendLine(user.Login);
            userRole.AppendLine(user.Role);

            return new JsonResult(response);
        }

        /*[Authorize]
        [HttpGet("/login", Name = "GetLogin")]
        public IActionResult GetLogin()
        {
            
            return new OkObjectResult("Ваш логин - " + userLogin.ToString());
        }

        [Authorize]
        [HttpGet("/role", Name = "GetRole")]
        public IActionResult GetRole()
        {
            if(userRole.ToString() == null && userLogin.ToString() == null)
                return new OkObjectResult("Ваш статус - администратор");
            return new OkObjectResult(userRole.ToString() + " + " + userLogin.ToString());
        }*/
    }

    public class AuthOptions
    {
        public const string ISSUER = "SuperheroesServer"; // издатель токена
        public const string AUDIENCE = "Superheroes"; // потребитель токена
        const string KEY = "mysupersecret_secretsecretsecretkey!123";   // ключ для шифрации
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
}
