using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Azure;
using BusinessObjects;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using LoginModel = BusinessObjects.Models.LoginModel;
using RegisterModel = BusinessObjects.Models.RegisterModel;

namespace API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    var role = await _roleManager.FindByNameAsync(userRole);
                    if (role != null)
                    {
                        var roleClaims = await _roleManager.GetClaimsAsync(role);
                        foreach (Claim roleClaim in roleClaims)
                        {
                            authClaims.Add(roleClaim);
                        }
                    }
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
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
            // Check if the Customer role exists
            if (!await _roleManager.RoleExistsAsync("Customer"))
            {
                // Create the Customer role
                var role = new IdentityRole("Customer");
                await _roleManager.CreateAsync(role);
            }

            // Add the user to the Customer role
            await _userManager.AddToRoleAsync(user, "Customer");

            return Ok(new ResponseModel() { Status = "Success", Message = "User created successfully!" });
        }

        //[HttpPost]
        //[Route("register-admin")]
        //public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        //{
        //    var userExists = await _userManager.FindByNameAsync(model.Email);
        //    if (userExists != null)
        //        return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User already exists!" });

        //    IdentityUser user = new IdentityUser()
        //    {
        //        Email = model.Email,
        //        SecurityStamp = Guid.NewGuid().ToString(),
        //        UserName = model.Email
        //    };
        //    var result = await _userManager.CreateAsync(user, model.Password);
        //    if (!result.Succeeded)
        //        return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User creation failed! Please check user details and try again." });

        //    if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
        //        await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
        //    if (!await _roleManager.RoleExistsAsync(UserRoles.Staff))
        //        await _roleManager.CreateAsync(new IdentityRole(UserRoles.Staff));
        //    if (!await _roleManager.RoleExistsAsync(UserRoles.Customer))
        //        await _roleManager.CreateAsync(new IdentityRole(UserRoles.Customer));

        //    if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
        //    {
        //        await _userManager.AddToRoleAsync(user, UserRoles.Admin);
        //    }

        //    return Ok(new ResponseModel { Status = "Success", Message = "User created successfully!" });
        //}
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
        //        { "picture", model.Picture },
        //        { "iss", "https://localhost:44300" },
        //        { "aud", "https://localhost:44300" },
        //        { "exp", DateTimeOffset.UtcNow.AddHours(3).ToUnixTimeSeconds() }
        //    };

        //    var token = new JwtSecurityToken(new JwtHeader(new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])), SecurityAlgorithms.HmacSha256)), payload);

        //    return Ok(new
        //    {
        //        token = new JwtSecurityTokenHandler().WriteToken(token),
        //        expiration = token.ValidTo
        //    });
        //}
        //[HttpGet]
        //[Route("check")]
        //public IActionResult SomeActionMethod()
        //{
        //    var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        //    int check;
        //    if (userRole == "Admin")
        //    {
        //        // User is an admin
        //        // Perform some action or return some view
        //        check = 1;
        //    }
        //    else if (userRole == "Staff")
        //    {
        //        // User is a staff member
        //        // Perform some action or return some view
        //        check = 2;
        //    }
        //    else if (userRole == "Customer")
        //    {
        //        // User is a customer
        //        // Perform some action or return some view
        //        check = 3;
        //    }
        //    else
        //    {
        //        return Unauthorized(); // If user role doesn't match any expected roles
        //    }
        //    return Ok(check);
        //}

    }
}
