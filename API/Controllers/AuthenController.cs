using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Azure;
using BusinessObjects;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Services;
using LoginModel = BusinessObjects.Models.LoginModel;
using RegisterModel = BusinessObjects.Models.RegisterModel;
using Repositories.Helper;
using Services.Interface;
using System.Diagnostics;
using Microsoft.AspNetCore.Cors;

namespace API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly TokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly Services.Interface.IMailService _mailService;

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
            try
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var userRole = roles.FirstOrDefault();
                    return Ok(new
                    {
                        Token = _tokenService.GenerateToken(user, userRole)
                    }
                    
                    );
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
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
            //return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel() { Status = "Error", Message = "User creation failed! Please check user details and try again." });
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
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            }

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
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            if (!await _roleManager.RoleExistsAsync(UserRoles.Staff))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Staff));

            if (await _roleManager.RoleExistsAsync(UserRoles.Staff))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Staff);
            }

            return Ok(new ResponseModel { Status = "Success", Message = "Staff created successfully!" });
        }

        // externalLogin like google and facebook
        //[HttpPost]
        //[Route("external-login")]
        //public async Task<IActionResult> ExternalLogin([FromBody] ExternalLoginModel model)
        //{
        //    var payload = new JwtPayload
        //    {
        //        { "sub", model.Email },
        //        { "email", model.Email },
        //        { "name", model.Name },
        //        //{ "picture", model.Picture },
        //        { "iss", "https://localhost:7104" },
        //        { "aud", "https://localhost:7104" },
        //        { "exp", DateTimeOffset.UtcNow.AddHours(3).ToUnixTimeSeconds() },
        //        { ClaimTypes.Role, "Customer" } // Add the first role as a claim
        //    };

        //    var token = new JwtSecurityToken(new JwtHeader(new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])), SecurityAlgorithms.HmacSha256)), payload);

        //    return Ok(new
        //    {
        //        token = new JwtSecurityTokenHandler().WriteToken(token),
        //        expiration = token.ValidTo
        //    });
        //}

        //ForgetPassword
        [HttpPost]
        [Route("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User does not exist!" });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Authentication", new { token = Uri.EscapeDataString(token), email = Uri.EscapeDataString(user.Email) }, Request.Scheme);
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

            // Giải mã token và email
            var decodedToken = Uri.UnescapeDataString(model.Token);
            var decodedEmail = Uri.UnescapeDataString(model.Email);

            var isTokenValid = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", decodedToken);
            Console.WriteLine("Is Token Valid: " + isTokenValid);

            if (!isTokenValid)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "Invalid token." });
            }

            var resetPassResult = await _userManager.ResetPasswordAsync(user, decodedToken, model.Password);
            if (!resetPassResult.Succeeded)
            {
                var errors = resetPassResult.Errors.Select(e => e.Description);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = string.Join(" ", errors) });
            }

            return Ok(new ResponseModel { Status = "Success", Message = "Password has been reset successfully!" });
        }

    }
}
