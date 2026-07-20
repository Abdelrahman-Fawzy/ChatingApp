using ChatingApp.Data;
using ChatingApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UsersController(AppDbContext context) { 
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<AppUser>>> GetMembers()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetMember(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(new {Message = "لا يوجد مستخدم"});
            return Ok(user);
        }
    }
}
