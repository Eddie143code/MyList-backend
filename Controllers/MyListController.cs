
using MyList_backend.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using MyList_backend.ViewModels;

namespace MyList_backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MyListController : ControllerBase
    {
        private readonly MyListDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public MyListController(MyListDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // POST: api/MyList
        [HttpPost]
        public async Task<ActionResult> Create(CreateMyListViewModel myList)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);

                MyList newMyList = new MyList { Name = myList.Name, User = currentUser };
                 _db.MyLists?.Add(newMyList);
                _db.SaveChanges();
                return Ok("success");

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message, InnerError = ex.InnerException?.Message });
            }
 


        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MyList>>> GetMyLists()
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);

                if (currentUser == null)
                {
                    // User not found, return a 404 NotFound result or another appropriate response
                    return NotFound();
                }

                List<MyList> userItems = _db.MyLists
                    .Where(entry => entry.User.Id == currentUser.Id)
                    .ToList();

                // Return the user-specific items as part of the ActionResult
                return Ok(userItems);
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message, InnerError = ex.InnerException?.Message });
            }
        }
    }
}