using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Azure;
using BusinessObjects;
using DAOs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Services;
using Repositories.Helper;
using Services.Interface;







namespace API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly TokenService _tokenService;
        private readonly UserService _userService = new UserService();
        private readonly UserDetailService _userDetailService = new UserDetailService();
        private readonly IConfiguration _configuration;
        private readonly IMailService _mailService;

        public AuthenticationController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IMailService mailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _tokenService = new TokenService(_configuration, _roleManager);
            _mailService = mailService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            //var ip = Utils.GetIpAddress(HttpContext);
            var user = await _userManager.FindByNameAsync(model.Email);

            if (user == null)
            {
                return
                    StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User not found!" });
            }

            //check if user is banned
            //if (BanList.BannedUsers.Contains(ip) || userDetail.Status == false)
            if (user.LockoutEnabled == false)
            {
                return
                    StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User is banned!" });
            }
            else
            {
                try
                {
                    if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        var userRole = roles.FirstOrDefault();
                        return Ok(new
                        {
                            Token = _tokenService.GenerateToken(user, userRole)
                        });
                    }
                    else
                    {
                        user.AccessFailedCount++;
                        if (user.AccessFailedCount == 5)
                        {
                            //BanList.BannedUsers.Add(ip);
                            user.LockoutEnabled = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest();
                }
            }

            return Unauthorized();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Email);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User already exists!" });

            IdentityUser user = new IdentityUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Email
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel { Status = "Error", Message = string.Join(" ", errors) });
            }

            if (!await _roleManager.RoleExistsAsync("Customer"))
            {
                var role = new IdentityRole("Customer");
                await _roleManager.CreateAsync(role);
            }

            await _userManager.AddToRoleAsync(user, "Customer");
            UserDetail userDetail = new UserDetail()
            {
                UserId = user.Id,
                Point = 0,
                FullName = model.FullName,
                ProfilePicture = $"https://firebasestorage.googleapis.com/v0/b/court-callers.appspot.com/o/user.jpg?alt=media&token=3601d057-9503-4cc8-b203-2eb0b89f900d"
                
            };
            _userDetailService.AddUserDetail(userDetail);
            return Ok(new ResponseModel() { Status = "Success", Message = "User created successfully!" });
        }





        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Email);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User already exists!" });

            IdentityUser user = new IdentityUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Email
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel { Status = "Error", Message = string.Join(" ", errors) });
            }

            if (!await _roleManager.RoleExistsAsync("Admin"))
                await _roleManager.CreateAsync(new IdentityRole("Admin"));


            await _userManager.AddToRoleAsync(user, "Admin");

            UserDetail userDetail = new UserDetail()
            {
                UserId = user.Id,
                Point = 0,
                FullName = model.FullName,

            };
            _userDetailService.AddUserDetail(userDetail);

            return Ok(new ResponseModel { Status = "Success", Message = "User created successfully!" });
        }





        [HttpPost]
        [Route("register-staff")]
        public async Task<IActionResult> RegisterStaff([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Email);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User already exists!" });

            IdentityUser user = new IdentityUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Email
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel { Status = "Error", Message = string.Join(" ", errors) });
            }

            if (!await _roleManager.RoleExistsAsync("Staff"))
                await _roleManager.CreateAsync(new IdentityRole("Staff"));

            await _userManager.AddToRoleAsync(user, "Staff");

            UserDetail userDetail = new UserDetail()
            {
                UserId = user.Id,
                Point = 0,
                FullName = model.FullName,

            };
            _userDetailService.AddUserDetail(userDetail);

            return Ok(new ResponseModel { Status = "Success", Message = "Staff created successfully!" });
        }



        [HttpPost]
        [Route("google-login")]
        public async Task<IActionResult> GoogleLogin(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            var email = jsonToken.Claims.First(claim => claim.Type == "email").Value;
            var name = jsonToken.Claims.First(claim => claim.Type == "name").Value;
            var picture = jsonToken.Claims.First(claim => claim.Type == "picture").Value;

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new IdentityUser
                {
                    Email = email,
                    UserName = email,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                await _userManager.CreateAsync(user);
                UserDetail userDetail = new UserDetail()
                {
                    UserId = user.Id,
                    Point = 0,
                    FullName = name,
                    ProfilePicture = picture
                    //Id = user.Id
                };
                _userDetailService.AddUserDetail(userDetail);
                await _userManager.AddToRoleAsync(user, "Customer");

            }

            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault();

            return Ok(new
            {
                Token = _tokenService.GenerateToken(user, userRole)
            });
        }

        //Facebook Login
        [HttpPost]
        [Route("facebook-login")]
        public async Task<IActionResult> FacebookLogin(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            var email = jsonToken.Claims.First(claim => claim.Type == "email").Value;
            var name = jsonToken.Claims.First(claim => claim.Type == "name").Value;
            var picture = jsonToken.Claims.First(claim => claim.Type == "picture").Value;

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new IdentityUser
                {
                    Email = email,
                    UserName = email,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                await _userManager.CreateAsync(user);
                UserDetail userDetail = new UserDetail()
                {
                    UserId = user.Id,
                    Point = 0,
                    FullName = name,
                    ProfilePicture = picture
                };
                _userDetailService.AddUserDetail(userDetail);
                await _userManager.AddToRoleAsync(user, "Customer");

            }

            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault();

            return Ok(new
            {
                Token = _tokenService.GenerateToken(user, userRole)
            });
        }




        //ForgetPassword
        [HttpPost]
        [Route("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User does not exist!" });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Authentication", new { token, email = user.Email }, Request.Scheme);
            Console.WriteLine("Generated Token: " + token);
            var mailRequest = new MailRequest
            {
                ToEmail = user.Email,
                Subject = "Court Caller Confirmation Email (Reset Password)",
                Body = API.Helper.FormEmail.EnailContent(user.Email, callbackUrl)
            };
            await _mailService.SendEmailAsync(mailRequest);

            return Ok(new ResponseModel { Status = "Success", Message = "Reset password link has been sent to your email address." });
        }


        //ResetPassword
        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User does not exist!" });

            var resetPassResult = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            var isTokenValid = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", model.Token);
            Console.WriteLine("Is Token Valid: " + isTokenValid);

            if (!resetPassResult.Succeeded)
            {
                var errors = resetPassResult.Errors.Select(e => e.Description);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = string.Join(" ", errors) });
            }

            return Ok(new ResponseModel { Status = "Success", Message = "Password has been reset successfully!" });
        }

    }
}
