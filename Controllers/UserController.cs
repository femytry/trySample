using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using trySample.Services;
using trySample.Dtos;
using trySample.Entities;
using trySample.Helpers;

namespace trySample.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IHostingEnvironment _environment;

        public UserController(IUserService userService, IMapper mapper, IOptions<AppSettings> appSettings, IHostingEnvironment environment)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _environment = environment;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Authenticate([FromForm]string username, [FromForm]string password)
        {
            var user = _userService.Authenticate(username, password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            _userService.StoreToken(user, tokenString);

            // return basic user info (without password) and token to store client side
            return Ok(new {
                Id = user.Id,
                Username = user.Username,
                Token = tokenString
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromForm]UserDto userDto)
        {
            // map dto to entity
            var user = _mapper.Map<User>(userDto);

            try 
            {
                // save 
                _userService.Create(user, userDto.Password);
                return Ok();
            } 
            catch(AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("account")]
        public IActionResult GetById()
        {
            Guid id = GetId();
            var user =  _userService.GetById(id);
            var userDto = _mapper.Map<UserResponseDto>(user);
            return Ok(userDto);
        }

        [HttpPut("update")]
        public IActionResult Update([FromForm]UserDto userDto)
        {
            // map dto to entity and set id
            Guid id = GetId();
            var user = _mapper.Map<User>(userDto);
            user.Id = id;

            try 
            {
                // save 
                _userService.Update(user, userDto.Password);
                return Ok();
            } 
            catch(AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public IActionResult Delete()
        {
            Guid id = GetId();
            _userService.Delete(id);
            return Ok();
        }

        [HttpPut("logout")]
        public IActionResult Logout([FromHeader]string Token)
        {
            _userService.Logout(Token);
            return Ok();
        }

        private Guid GetId(){
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            Guid id = Guid.Parse(identity.FindFirst(ClaimTypes.Name).Value);
            return id;
        } 
        
    }
}
