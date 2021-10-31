using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MonamourWeb.Models
{
    public class Procedure
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Процедура")]
        public string Title { get; set; }
        
        [DisplayName("Животное")]
        public Animal Animal { get; set; }
        
        [Required]
        public int AnimalId { get; set; }

        [DisplayName("Стоимость")]
        public double? Cost { get; set; }

        [DisplayName("Инфо")]
        public string Info { get; set; }
    }
}
