using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MonamourWeb.Models
{
    public class VisitStatus
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Статус")]
        public string Status { get; set; }
    }
}