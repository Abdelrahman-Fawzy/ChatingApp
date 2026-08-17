using ChatingApp.BackEnd.Controllers;
using ChatingApp.BackEnd.Entities;
using ChatingApp.BackEnd.Interfaces;
using ChatingApp.Data;
using ChatingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ChatingApp.Controllers
{
    [Authorize]
    public class MembersController(IMemberRepository memberRepository) : BaseApiController
    {

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Member>>> GetMembers()
        {
            return Ok(await memberRepository.GetMembersAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Member>> GetMember(string id)
        {
            var user = await memberRepository.GetMember(id);
            if (user == null) return NotFound(new {Message = "لا يوجد مستخدم"});
            return Ok(user);
        }

        [HttpGet("{id}/photos")]
        public async Task<ActionResult<IReadOnlyList<Photo>>> GetPhotosByMemberId(string id)
        {
            return Ok(await memberRepository.GetPhotosByMemberIdAsync(id));
        }
    }
}
