using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseAPIController
    {
        private readonly DataContext _context; // DI to add database
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService)
        {
            // injecting token service to controller
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("register")] //endpoint to register the users to application
        // string username and string password can not be used as parameters, cos the request made 
        // from body should be recieved as objects instead of strings hence we use DTO here

        // status code 400 is bad request that we get from actionresult
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if(await UserExists(registerDto.Username)) return BadRequest("UserName is taken");

            using var hmac = new HMACSHA512(); // ANY class that drives from IDisposable interface should provide 
            // a dispose method, so we are using using keyword to guarantee, as soon as we finish using this
            //class it is disposed.

            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user); // adds the user to User table in EF not to db
            await _context.SaveChangesAsync(); // this method adds changes to db

            // previously this was returning user and that is not correct we need to return a jwt token, so we will create a userdto to return the token
            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        // Helper method to check for user and it is only available in this class
        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }

        // checks for user another dto has to be created can not use same as register dto
        // should decrypt the encrypted password
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _context.Users
            .SingleOrDefaultAsync(x => x.UserName == loginDto.Username);

            if(user == null) return Unauthorized("Invalid Username");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            // this hash shud be same as hasd in db
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
            }

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }
    }
}