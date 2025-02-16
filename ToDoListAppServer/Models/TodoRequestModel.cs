using System.ComponentModel.DataAnnotations;

namespace ToDoListAppServer.Models
{
    public class TodoRequestModel
    {
        [Required]
        public string? Title { get; set; }

        public string? Description { get; set; }
    }
}
