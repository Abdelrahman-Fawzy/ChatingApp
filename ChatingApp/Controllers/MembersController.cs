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
    public class MembersController(IMemberRepository memberRepository, IPhotoService _photoService) : BaseApiController
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

        [HttpPost("add-photo")]
        public async Task<ActionResult<Photo>> AddPhoto(IFormFile file)
        {
            var member = await memberRepository.GetMemberToUpdate(User.GetMemberId());

            if (member == null) return BadRequest("Cannot update member");

            var result = await _photoService.UploadPhotoAync(file);

            if(result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
                MemberId = User.GetMemberId()
            };

            if(member.ImageUrl == null)
            {
                member.ImageUrl = photo.Url;
                member.User.ImageUrl = photo.Url;
            }

            member.Photos.Add(photo);

            if(await  memberRepository.SaveChangesAsync() ) return photo;

            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoID}")]
        public async Task<ActionResult> SetMainPhoto(int photoID)
        {
            var member = await memberRepository.GetMemberToUpdate(User.GetMemberId());

            if (member == null) return BadRequest("Cannot get member from token");

            var photo = member.Photos.SingleOrDefault(p => p.Id == photoID);

            if (photo == null) return BadRequest("هذا الصورة غير موجودة");
            if (member.ImageUrl == photo.Url) return BadRequest("هذه الصورة هي الرئيسية بالفعل");

            member.ImageUrl = photo.Url;
            member.User.ImageUrl = photo.Url;

            if (await memberRepository.SaveChangesAsync()) return NoContent();

            return BadRequest("Problem setting main photo");
        }

        [HttpDelete("delete-photo/{photoID}")]
        public async Task<ActionResult> DeletePhoto(int photoID)
        {
            var member = await memberRepository.GetMemberToUpdate(User.GetMemberId());

            if (member == null) return BadRequest("Cannot get member from token");

            var photo = member.Photos.SingleOrDefault(p => p.Id == photoID);

            if (photo == null) return BadRequest("هذا الصورة غير موجودة");
            if (member.ImageUrl == photo.Url) return BadRequest("لا يمكن حذف الصورة الرئيسية");

            if(photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            member.Photos.Remove(photo);

            if (await memberRepository.SaveChangesAsync()) return NoContent();

            return BadRequest("Problem deleting the photo");
        }
    }
}
