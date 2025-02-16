using System.ComponentModel.DataAnnotations;

namespace ToDoListAppServer.Models
{
    public class Todo
    {
        [Key]
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public string? UserId { get; set; }
    }
}
