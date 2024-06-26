﻿using DataBase.BD;
using DataBase.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using UserService.AuthorizationModel;

namespace UserService.Controllers
{
    public static class RSATools
    {
        public static RSA GetPrivateKey(string path = "rsa/private_key.pem")
        {
            var privateKey = LoadPrivateKeyFromFile(path);
            return CreateRSAFromPem(privateKey);
        }

        private static string LoadPrivateKeyFromFile(string filePath)
        {
            // загрузка закрытого ключа из файла
            return File.ReadAllText(filePath);
        }

        private static RSA CreateRSAFromPem(string pem)
        {
            // создание объекта RSA из PEM-формата
            var rsa = RSA.Create();
            rsa.ImportFromPem(pem);
            return rsa;
        }
    }

    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepository;

        public LoginController(IConfiguration config, IUserRepository userRepository)
        {
            _config = config;
            _userRepository = userRepository;
        }

        private UserRole RoleIDToRole(RoleId roleId)
        {
            if (roleId == RoleId.Admin) return UserRole.Administrator;
            return UserRole.User;
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login([FromBody] LoginModel userLogin)
        {
            try
            {
                var roleId = _userRepository.UserCheck(userLogin.Name, userLogin.Password);
                var user = new UserModel { Username = userLogin.Name, Role = RoleIDToRole(roleId) };
                var token = GenerateToken(user);
                return Ok(token);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("AddAdmin")]
        public ActionResult AddAdmin([FromBody] LoginModel userLogin)
        {
            try
            {
                _userRepository.UserAdd(userLogin.Name, userLogin.Password, RoleId.Admin);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
            return Ok();
        }

        [HttpPost]
        [Route("AddUser")]
        [Authorize(Roles = "Administrator")]
        public ActionResult AddUser([FromBody] LoginModel userLogin)
        {
            try
            {
                _userRepository.UserAdd(userLogin.Name, userLogin.Password, RoleId.User);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
            return Ok();
        }

        private string GenerateToken(UserModel user)
        {
            var securityKey = new RsaSecurityKey(RSATools.GetPrivateKey());
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256Signature);
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet]
        [Route("GetUserId")]
        [Authorize]
        public ActionResult GetUserId()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaims = identity.Claims;
                return Ok(userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);
            }
            return Unauthorized();
        }
    }
}