using ChatingApp.BackEnd.Controllers;
using ChatingApp.BackEnd.DTOs;
using ChatingApp.BackEnd.Entities;
using ChatingApp.BackEnd.Extensions;
using ChatingApp.BackEnd.Interfaces;
using ChatingApp.Data;
using ChatingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;

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

        [HttpPut()]
        public async Task<ActionResult<Member>> UpdateMember(MemberUpdateDTO memberUpdateDTO)
        {
            var memberId = User.GetMemberId();

            var member = await memberRepository.GetMemberToUpdate(memberId);

            if(member == null) return BadRequest("Could not get member");

            member.DisplayName = memberUpdateDTO.DisplayName ?? member.DisplayName;
            member.Description = memberUpdateDTO.Description ?? member.Description;
            member.Country = memberUpdateDTO.Country ?? member.Country;
            member.City = memberUpdateDTO.City ?? member.City;

            memberRepository.Update(member);

            if (await memberRepository.SaveChangesAsync())
                return NoContent();

            return BadRequest("Failed to update member");

        }
    }
}
