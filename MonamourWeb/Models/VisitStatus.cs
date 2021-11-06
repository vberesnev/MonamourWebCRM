using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MonamourWeb.Models
{
    public class VisitStatus : ICloneable
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Статус посещения не может быть пустой")]
        [DisplayName("Статус")]
        public string Status { get; set; }

        public override string ToString()
        {
            return $"Статут посещения: [Id]: {Id}; [Статус]: {Status};";
        }

        public object Clone()
        {
            return new VisitStatus()
            {
                Id = this.Id,
                Status = this.Status
            };
        }
    }
}