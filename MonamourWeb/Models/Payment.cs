using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MonamourWeb.Models
{
    public class Payment
    {
        public int Id { get; set; }

        [Required]
        public int PaymentTypeId { get; set; }

        [DisplayName("Тип оплаты")]
        public PaymentType PaymentType { get; set; }

        [Required]
        [DisplayName("Сумма")]
        public int Sum { get; set; }

        [Required]
        public int VisitId { get; set; }

        [DisplayName("Визит")]
        public Visit Visit { get; set; }
    }
}