using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyList_backend.Model;
using MyList_backend.ViewModels;
using System.Security.Claims;

namespace MyList_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AccountController  : ControllerBase
    {
        private readonly MyListDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, MyListDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterViewModel model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest("Invalid data format");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                ApplicationUser user = new ApplicationUser { UserName = model.Email };
                IdentityResult result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return Ok(result);
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception accordingly
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginViewModel model)
        {

            try
            {

                if (model == null)
                {
                    return BadRequest("Invalid data format");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);

                }

                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: true, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return Ok("Login successful"); // Replace with appropriate response
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt");
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception accordingly
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("logout")]
        public async Task<ActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return Ok("Logout successful");
        }

    }
}
