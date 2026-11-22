using System.ComponentModel.DataAnnotations;

namespace ChatingApp.BackEnd.Entities
{
    public class Group(string name)
    {
        [Key]
        public string Name { get; set; } = name;
        public ICollection<Connection> Connections { get; set; } = [];
    }
}
