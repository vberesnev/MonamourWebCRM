using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MonamourWeb.Models
{
    public class Procedure : ICloneable
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Процедура должна иметь название")]
        [DisplayName("Процедура")]
        public string Title { get; set; }
        
        [DisplayName("Животное")]
        public Animal Animal { get; set; }
        
        [Required(ErrorMessage = "Процедура должна относиться к животному")]
        public int AnimalId { get; set; }

        [DisplayName("Стоимость")]
        public double? Cost { get; set; }

        [DisplayName("Инфо")]
        public string Info { get; set; }

        [DisplayName("Ориентировочное время")]
        public int? ApproximateTime { get; set; }

        public override string ToString()
        {
            return $"Процедура: [Id]: {Id}; [Процедура]: {Title}; [Id животного]: {AnimalId}; [Животное]: {Animal?.Title}; [Стоимость]: {Cost : 0};" +
                   $" [Инфо]: {Info}; [Ориентировочное время]: {ApproximateTime};";
        }

        public object Clone()
        {
            return new Procedure()
            {
                Id = this.Id,
                Title = this.Title,
                AnimalId = this.AnimalId,
                Animal = this.Animal?.Clone() as Animal,
                Cost = this.Cost,
                Info = this.Info,
                ApproximateTime = this.ApproximateTime
            };
        }
    }
}
