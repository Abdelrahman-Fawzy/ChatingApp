using ChatingApp.BackEnd.Controllers;
using ChatingApp.Data;
using ChatingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatingApp.Controllers
{
    public class MembersController : BaseApiController
    {
        private readonly AppDbContext _context;
        public MembersController(AppDbContext context) { 
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<AppUser>>> GetMembers()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<AppUser>> GetMember(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(new {Message = "لا يوجد مستخدم"});
            return Ok(user);
        }
    }
}
