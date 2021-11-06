using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MonamourWeb.Models
{
    public class PaymentType : ICloneable
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Тип оплаты должен быть заполнен")]
        [DisplayName("Тип оплаты")]
        public string Type { get; set; }

        public override string ToString()
        {
            return $"Тип оплаты: [Id]: {Id}; [Тип]: {Type};";
        }

        public object Clone()
        {
            return new PaymentType()
            {
                Id = this.Id,
                Type = this.Type
            };
        }
    }
}