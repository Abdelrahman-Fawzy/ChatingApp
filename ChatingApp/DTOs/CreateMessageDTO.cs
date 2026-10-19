namespace ChatingApp.BackEnd.DTOs
{
    public class CreateMessageDTO
    {
        public required string RecipientId { get; set; }
        public required string Content { get; set; }
    }
}
