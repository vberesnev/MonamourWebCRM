using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MonamourWeb.Models
{
    public class VisitProcedure
    {
        public int Id { get; set; }
        
        [Required]
        public int ProcedureId { get; set; }

        [DisplayName("Процедура")]
        public Procedure Procedure { get; set; }
        
        [Required]
        [DisplayName("Стоимость")]
        public int Cost { get; set; }

        [Required]
        public int VisitId { get; set; }

        [DisplayName("Посещение")]
        public Visit Visit { get; set; }
    }
}