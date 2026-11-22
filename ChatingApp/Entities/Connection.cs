namespace ChatingApp.BackEnd.Entities
{
    public class Connection(string connectionID, string userID)
    {
        public string ConnectionID { get; set; } = connectionID;
        public string UserID { get; set; } = userID;

        public Group Group { get; set; } = null!;
    }
}
