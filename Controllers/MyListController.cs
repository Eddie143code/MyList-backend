
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

        [HttpPost("{myListName}/items")]
        public async Task<ActionResult<Item>> CreateItem(string myListName, [FromBody] CreateItemViewModel item)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                    return Unauthorized("User not authenticated");

                var myList = _db.MyLists
                    .FirstOrDefault(entry => entry.Name == myListName && entry.User.Id == currentUser.Id);

                if (myList == null)
                    return NotFound("MyList not found or unauthorized access");

                var newItem = new Item { Name = item.Name, User = currentUser };
                myList.Items ??= new List<Item>();
                myList.Items.Add(newItem);

                _db.SaveChanges();

                return Ok(newItem);
            }
            catch (Exception ex)
            {
                // Log the exception details using your preferred logging mechanism
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message, InnerError = ex.InnerException?.Message });
            }
        }


        [HttpGet("{myListName}/items")]
        public async Task<ActionResult<IEnumerable<Item>>> GetItemsInList(string myListName)
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);

                // Ensure the requested MyList belongs to the current user
                MyList myList = _db.MyLists
                    .FirstOrDefault(entry => entry.Name == myListName && entry.User.Id == currentUser.Id);

                if (myList == null)
                {
                    // MyList not found or doesn't belong to the user
                    return NotFound("MyList not found or unauthorized access");
                }

                List<Item> itemsInList = _db.Items
                    .Where(item => item.User.Id == currentUser.Id && item.MyList.MyListId == myList.MyListId)
                    .ToList();

                return Ok(itemsInList);
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