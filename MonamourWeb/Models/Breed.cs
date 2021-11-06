using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MonamourWeb.Models
{
    public class Breed : ICloneable
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "У породы должно быть название")]
        [DisplayName("Порода")]
        public string Title { get; set; }
        
        [DisplayName("Животное")]
        public Animal Animal { get; set; }
        
        [Required(ErrorMessage = "Порода должны принадлежать животному")]
        public int AnimalId { get; set; }

        public override string ToString()
        {
            return $"Порода: [Id]: {Id}; [Порода]: {Title}; [Id животного]: {AnimalId}; [Животное]: {Animal?.Title}";
        }

        public object Clone()
        {
            return new Breed()
            {
                Id = this.Id,
                Title = this.Title,
                Animal = this.Animal?.Clone() as Animal,
                AnimalId = this.AnimalId
            };
        }
    }
}