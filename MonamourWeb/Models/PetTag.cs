using System.Collections.Generic;

namespace MonamourWeb.Models
{
    public class PetTag : Tag
    {
        public ICollection<Pet> Pets { get; set; }
        
        public PetTag()
        {
            Pets = new List<Pet>();
        }

        public override string ToString()
        {
            return $"Тег питомцев: [Id]: {Id}; [Тег]: {Title}; [Короткий тег]: {ShortTitle}; [Цвет]: {Color};";
        }
        
        public override object Clone()
        {
            return new PetTag()
            {
                Id = this.Id,
                Color = this.Color,
                Title = this.Title,
                ShortTitle = this.ShortTitle
            };
        }
    }
}