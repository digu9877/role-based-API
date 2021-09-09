using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Model;
using WebApplication6.Models;


namespace WebApplication6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUserController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationSettings _appSettings;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWTConfig _jWTConfig;

        public ApplicationUserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IOptions<ApplicationSettings> appSettings, RoleManager<IdentityRole> roleManager, IOptions<JWTConfig> jwtConfig)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appSettings = appSettings.Value;
            _roleManager = roleManager;
            _jWTConfig = jwtConfig.Value;
        }

       // [HttpPost]
       // [Route("Register")]
       //// POST : /api/ApplicationUser/Register
       // public async Task<Object> PostApplicationUser(ApplicationUserModel model)
       // {
            
       //     var applicationUser = new ApplicationUser()
       //     {
       //         UserName = model.UserName,
       //         Email = model.Email,
       //         FullName = model.FullName


       //     };

       //     try
       //     {

       //         var result = await _userManager.CreateAsync(applicationUser, model.Password);

       //         if (result.Succeeded)
       //         {
       //             var tempUser = await _userManager.FindByEmailAsync(model.Email);

       //             await _userManager.AddToRoleAsync(tempUser, model.Role);
       //             //foreach (var role in model.Role)
       //             //{
       //             //    await _userManager.AddToRoleAsync(tempUser, role.ToString());
       //             //}
       //             return await Task.FromResult(new ResponseModel(ResponseCode.OK, "User has been Registered", null));
       //         }
       //         return await Task.FromResult(new ResponseModel(ResponseCode.Error, "", result.Errors.Select(x => x.Description).ToArray()));
       //     }
       //     catch (Exception ex)
       //     {
       //         return await Task.FromResult(new ResponseModel(ResponseCode.Error, ex.Message, null));
       //     }
       // }

        [HttpPost("RegisterUser")]
        public async Task<object> RegisterUser([FromBody] ApplicationUserModel model)
        {
            try
            {
                if (model.Role == null)
                {
                    return await Task.FromResult(new ResponseModel(ResponseCode.Error, "Roles are missing", null));
                }
                //foreach (var role in model.Role)
                //{

                //    if (!await _roleManager.RoleExistsAsync(role.ToString()))
                //    {

                //        return await Task.FromResult(new ResponseModel(ResponseCode.Error, "Role does not exist", null));
                //    }
                //}


                var user = new ApplicationUser() { FullName = model.FullName, Email = model.Email, UserName = model.Email};
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var tempUser = await _userManager.FindByEmailAsync(model.Email);
                    await _userManager.AddToRoleAsync(tempUser, model.Role);
                    //foreach (var role in model.Role)
                    //{
                    //    await _userManager.AddToRoleAsync(tempUser, role.ToString());
                    //}
                    return await Task.FromResult(new ResponseModel(ResponseCode.OK, "User has been Registered", null));
                }
                return await Task.FromResult(new ResponseModel(ResponseCode.Error, "", result.Errors.Select(x => x.Description).ToArray()));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new ResponseModel(ResponseCode.Error, ex.Message, null));
            }
        }






       // [HttpPost]
       // [Route("Login")]
       //// POST : /api/ApplicationUser/Login
       // public async Task<IActionResult> Login(LoginModel model)
       // {
       //     var user = await _userManager.FindByNameAsync(model.UserName);
       //     if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
       //     {
       //         var tokenDescriptor = new SecurityTokenDescriptor
       //         {
       //             Subject = new ClaimsIdentity(new Claim[]
       //             {
       //                 new Claim("UserID",user.Id.ToString())
       //             }),
       //             Expires = DateTime.UtcNow.AddDays(1),
       //             SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Jwt_Secret)), SecurityAlgorithms.HmacSha256Signature)
       //         };
       //         var tokenHandler = new JwtSecurityTokenHandler();
       //         var securityToken = tokenHandler.CreateToken(tokenDescriptor);
       //         var token = tokenHandler.WriteToken(securityToken);
       //         return Ok(new { token });
       //     }
       //     else
       //         return BadRequest(new { message = "Username or password is incorrect." });
       // }

        [HttpPost("Login")]
        public async Task<object> Login([FromBody] LoginModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
                    if (result.Succeeded)
                    {
                        var appUser = await _userManager.FindByEmailAsync(model.Email);
                        var role = (await _userManager.GetRolesAsync(appUser)).FirstOrDefault();
                        var user = new UserDTO(appUser.FullName, appUser.Email, appUser.UserName,role);
                        user.Token = GenerateToken(appUser,role);

                        return await Task.FromResult(new ResponseModel(ResponseCode.OK, "", user));

                    }
                }

                return await Task.FromResult(new ResponseModel(ResponseCode.Error, "invalid Email or password", null));

            }
            catch (Exception ex)
            {
                return await Task.FromResult(new ResponseModel(ResponseCode.Error, ex.Message, null));
            }
        }





        [Authorize(Roles = "Admin")]
        [HttpPost("AddRole")]
        public async Task<object> AddRole([FromBody] RoleModel model)
        {
            try
            {
                if (model == null || model.Role == "")
                {

                    return await Task.FromResult(new { message = "parameters are missing" });

                }
                if (await _roleManager.RoleExistsAsync(model.Role))
                {
                    return await Task.FromResult(new { message = "Role already exist" });

                }
                var role = new IdentityRole();
                role.Name = model.Role;
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {

                    return await Task.FromResult(new { message = "Role added successfully" });
                }

                return await Task.FromResult(new { message = "something went wrong please try again later" });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        ///<summary>
        ///Get All User from database   
        ///</summary>
        [Authorize(Roles="Admin")]
        [HttpGet("GetAllUser")]
        public async Task<object> GetAllUser()
        {
            try
            {
                List<UserDTO> allUserDTO = new List<UserDTO>();
                var users = _userManager.Users.ToList();
                foreach (var user in users)
                {
                    var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

                    allUserDTO.Add(new UserDTO(user.FullName, user.Email, user.UserName, role));
                }
                return await Task.FromResult(new ResponseModel(ResponseCode.OK, "", allUserDTO));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new ResponseModel(ResponseCode.Error, ex.Message, null));
            }
        }



        [Authorize(Roles ="User,Admin")]
        [HttpGet("GetUser")]
        public async Task<object> GetUser()
        {
            try
            {
                List<UserDTO> allUserDTO = new List<UserDTO>();
                var users = _userManager.Users.ToList();
                foreach (var user in users)
                {
                    var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                    if (role == "User")
                    {
                        allUserDTO.Add(new UserDTO(user.FullName, user.Email, user.UserName, role));
                    }
                }
                return await Task.FromResult(new ResponseModel(ResponseCode.OK, "", allUserDTO));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new ResponseModel(ResponseCode.Error, ex.Message, null));
            }
        }


        [HttpGet("GetRoles")]
        public async Task<object> GetRoles()
        {
            try
            {

                var roles = _roleManager.Roles.Select(x => x.Name).ToList();

                return await Task.FromResult(new ResponseModel(ResponseCode.OK, "", roles));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new ResponseModel(ResponseCode.Error, ex.Message, null));
            }
        }




        private string GenerateToken(ApplicationUser user, string role)
        {
            var claims = new List<System.Security.Claims.Claim>(){
     new System.Security.Claims.Claim(JwtRegisteredClaimNames.NameId,user.Id),
               new System.Security.Claims.Claim(JwtRegisteredClaimNames.Email,user.Email),
               new System.Security.Claims.Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),

               new System.Security.Claims.Claim(ClaimTypes.Role,role),
           };
            //foreach (var role in roles)
            //{
            //    claims.Add(new System.Security.Claims.Claim(ClaimTypes.Role, role));
            //}

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jWTConfig.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(12),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = _jWTConfig.Audience,
                Issuer = _jWTConfig.Issuer
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }

    }
}