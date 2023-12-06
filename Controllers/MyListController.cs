
using MyList_backend.Model;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

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
        public async Task<ActionResult> Create(MyList myList)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                ApplicationUser currentUser = await _userManager.FindByIdAsync(userId);
                myList.User = currentUser;
                _db.MyLists?.Add(myList);
                _db.SaveChanges();
                return Ok("success");

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }

        
        }

        // Add other CRUD operations as needed
    }
}