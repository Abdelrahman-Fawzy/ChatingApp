using ChatingApp.BackEnd.DTOs;
using ChatingApp.BackEnd.Entities;
using ChatingApp.BackEnd.Extensions;
using ChatingApp.BackEnd.Helpers;
using ChatingApp.BackEnd.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatingApp.BackEnd.Controllers
{
    public class MessagesController(IMessageRepository messageRepository, IMemberRepository memberRepository) : BaseApiController
    {
        [HttpPost()]
        public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO createMessageDTO)
        {
            var sender = await memberRepository.GetMember(User.GetMemberId());
            var recepient = await memberRepository.GetMember(createMessageDTO.RecipientId);

            if(recepient == null)
                return BadRequest(new { Message = "هذا المستخدم غير موجود" });
            if(sender == null)
                return BadRequest(new { Message = "هذا المرسل غير موجود" });
            if(sender.Id == recepient.Id)
                return BadRequest(new { Message = "لا يمكنك إرسال رسالة إلى نفسك" });

            var message = new Message
            {
                SenderId = sender.Id,
                RecipientId = recepient.Id,
                Content = createMessageDTO.Content
            };

            messageRepository.AddMessage(message);

            if (await messageRepository.SaveAllAsync()) return MessageExtention.ToDTO(message);

            return BadRequest(new { Message = "فشل إرسال الرسالة" });
        }

        [HttpGet()]
        public async Task<ActionResult<PaginatedResult<MessageDTO>>> GetMessagesByContainer([FromQuery] MessageParams messageParams)
        {
            messageParams.MemberId = User.GetMemberId();
            return Ok(await messageRepository.GetMessagesForMember(messageParams));
        }

        [HttpGet("thread/{otherMemberId}")]
        public async Task<ActionResult<IReadOnlyList<MessageDTO>>> GetMessageThread(string otherMemberId)
        {
            return Ok(await messageRepository.GetMessageThread(User.GetMemberId(), otherMemberId));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(string id)
        {
            var memberId = User.GetMemberId();
            var message = await messageRepository.GetMessage(id);

            if(message == null) return NotFound(new { Message = "الرسالة غير موجودة" });

            if (message.SenderId != memberId && message.RecipientId != memberId)
                return Unauthorized(new { Message = "ليس لديك صلاحية حذف هذه الرسالة" });

            if (message.SenderId == memberId) message.SenderDeleted = true;
            if (message.RecipientId == memberId) message.RecipientDeleted = true;

            if (message.SenderDeleted && message.RecipientDeleted)
                messageRepository.DeleteMessage(message);

            if (await messageRepository.SaveAllAsync()) return Ok();

            return BadRequest(new { Message = "فشل حذف الرسالة" });
        }
    }
}
