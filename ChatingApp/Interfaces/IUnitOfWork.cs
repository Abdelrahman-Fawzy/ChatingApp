namespace ChatingApp.BackEnd.Interfaces
{
    public interface IUnitOfWork
    {
        IMemberRepository MemberRepository { get; }
        IMessageRepository MessageRepository { get; }
        ILikesRepository LikesRepository { get; }
        Task<bool> CompleteAsync();
        bool HasChanges();
    }
}
