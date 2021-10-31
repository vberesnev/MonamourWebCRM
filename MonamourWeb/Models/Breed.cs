using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MonamourWeb.Models
{
    public class Breed
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Порода")]
        public string Title { get; set; }
        
        [DisplayName("Животное")]
        public Animal Animal { get; set; }
        
        [Required]
        public int AnimalId { get; set; }
    }
}