using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MonamourWeb.Models
{
    public class PaymentType
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Тип оплаты")]
        public string Type { get; set; }
    }
}