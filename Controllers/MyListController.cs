﻿
using MyList_backend.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using MyList_backend.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace MyList_backend.Controllers
{
 
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class MyListController : ControllerBase
    {
        private readonly MyListDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly ILogger<MyListController> _logger;

     
        

        public MyListController(MyListDbContext db, UserManager<ApplicationUser> userManager, ILogger<MyListController> logger)
        {
            _db = db;
            _userManager = userManager;
            _logger = logger;
        }

        // list

        // POST: api/MyList
        [HttpPost]
        public async Task<ActionResult<MyList>> Create(CreateViewModel myList)
        {
         
         

            try
            {
                _logger.LogInformation("Request received in create list. User: {User}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);

                MyList newMyList = new MyList { Name = myList.Name, User = currentUser };
                 _db.MyLists?.Add(newMyList);
                _db.SaveChanges();
                return Ok(newMyList);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal fuck you" });
            }
        }

        // GET: api/MyList
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MyList>>> GetMyLists()
        {
            try {
                _logger.LogInformation("Request received. User: {User}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);

                List<MyList> userItems = _db.MyLists
             .Include(list => list.Items)  // Eager load the related items
             .Where(entry => entry.User.Id == currentUser.Id)
             .ToList();
                _logger.LogInformation("Request received. User's items: {userItems}",userItems);


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

        [HttpDelete("{myListId}")]
        public ActionResult DeleteMyList(int myListId)
        {
            try
            {
                MyList myList = _db.MyLists.Find(myListId);

                if (myList == null)
                {
                    return NotFound("MyList not found");
                }

                // Delete or dissociate associated items first
                _db.Items.RemoveRange(_db.Items.Where(item => item.MyListId == myListId));

                // Now, delete the MyList
                _db.MyLists.Remove(myList);
                _db.SaveChanges();

                return Ok(myListId);
            }
            catch (Exception ex)
            {
                // Log the exception details using your preferred logging mechanism
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message, InnerError = ex.InnerException?.Message });
            }
        }

        [HttpPut("{myListId}")]
        public async Task<ActionResult> UpdateMyList(int myListId, [FromBody] UpdateViewModel updatedMyList)
        {
            try
            {
               

                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);

                // Ensure the requested MyList belongs to the current user
                MyList myListToUpdate = _db.MyLists
                    .FirstOrDefault(entry => entry.MyListId == myListId && entry.User.Id == currentUser.Id);

                if (myListToUpdate == null)
                {
                    // MyList not found or doesn't belong to the user
                    return NotFound("MyList not found or unauthorized access");
                }

                // Update MyList properties
                myListToUpdate.Name = updatedMyList.Name;
                // Add more properties as needed

                _db.SaveChanges();

                return Ok("MyList updated successfully");
            }
            catch (Exception ex)
            {
                // Log the exception details using your preferred logging mechanism
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message, InnerError = ex.InnerException?.Message });
            }
        }



        // items


        [HttpPost("{myListId}/items")]
        public async Task<ActionResult> CreateItem(int myListId, [FromBody] CreateViewModel item)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                    return Unauthorized("User not authenticated");

                var myList = _db.MyLists
                    .FirstOrDefault(entry => entry.MyListId == myListId && entry.User.Id == currentUser.Id);

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


        [HttpGet("{myListId}/items")]
        public async Task<ActionResult<IEnumerable<Item>>> GetItemsInList(int myListId)
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);

                // Ensure the requested MyList belongs to the current user
                MyList myList = _db.MyLists
                    .FirstOrDefault(entry => entry.MyListId == myListId && entry.User.Id == currentUser.Id);

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

        [HttpDelete("{myListId}/items/{itemId}")]
        public async Task<ActionResult> DeleteItemInList(int myListId, int itemId)
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);

                // Ensure the requested MyList and Item belong to the current user
                MyList myList = _db.MyLists
                    .FirstOrDefault(entry => entry.MyListId == myListId && entry.User.Id == currentUser.Id);

                if (myList == null)
                {
                    // MyList not found or doesn't belong to the user
                    return NotFound("MyList not found or unauthorized access");
                }

                Item itemToDelete = _db.Items
                    .FirstOrDefault(item => item.ItemId == itemId && item.User.Id == currentUser.Id && item.MyList.MyListId == myList.MyListId);

                if (itemToDelete == null)
                {
                    return NotFound("Item not found in the specified list");
                }

                _db.Items.Remove(itemToDelete);
                _db.SaveChanges();

                return Ok(itemToDelete);
            }
            catch (Exception ex)
            {
                // Log the exception details using your preferred logging mechanism
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message, InnerError = ex.InnerException?.Message });
            }
        }

        [HttpPut("{myListId}/items/{itemId}")]
        public async Task<ActionResult> UpdateItemInList(int myListId, int itemId, [FromBody] UpdateViewModel updatedItem)
        {
            try
            {
         

                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);

                // Ensure the requested MyList and Item belong to the current user
                MyList myList = _db.MyLists
                    .FirstOrDefault(entry => entry.MyListId == myListId && entry.User.Id == currentUser.Id);

                if (myList == null)
                {
                    // MyList not found or doesn't belong to the user
                    return NotFound("MyList not found or unauthorized access");
                }

                Item itemToUpdate = _db.Items
                    .FirstOrDefault(item => item.ItemId == itemId && item.User.Id == currentUser.Id && item.MyList.MyListId == myList.MyListId);

                if (itemToUpdate == null)
                {
                    return NotFound("Item not found in the specified list");
                }

                // Update item properties
                itemToUpdate.Name = updatedItem.Name;
                // Add more properties as needed

                _db.SaveChanges();

                return Ok(itemToUpdate);
            }
            catch (Exception ex)
            {
                // Log the exception details using your preferred logging mechanism
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message, InnerError = ex.InnerException?.Message });
            }
        }


    }
}