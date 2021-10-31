using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MonamourWeb.Models
{
    public class Animal
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Животное")]
        public string Title { get; set; }
    }
}