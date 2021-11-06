using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MonamourWeb.Models
{
    public class Animal : ICloneable
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "У животного должно быть название")]
        [DisplayName("Животное")]
        public string Title { get; set; }

        public override string ToString()
        {
            return $"Животное: [Id]: {Id}; [Животное]: {Title}";
        }

        public object Clone()
        {
            return new Animal()
            {
                Id = this.Id,
                Title = this.Title
            };
        }
    }
}