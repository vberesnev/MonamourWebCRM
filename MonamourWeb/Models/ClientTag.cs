using System.Collections.Generic;

namespace MonamourWeb.Models
{
    public class ClientTag : Tag
    {
        public ICollection<Client> Clients { get; set; }
        
        public ClientTag()
        {
            Clients = new List<Client>();
        }

        public override string ToString()
        {
            return $"Клиентский тег: [Id]: {Id}; [Тег]: {Title}; [Короткий тег]: {ShortTitle}; [Цвет]: {Color};";
        }


        public override object Clone()
        {
            return new ClientTag()
            {
                Id = this.Id,
                Color = this.Color,
                Title = this.Title,
                ShortTitle = this.ShortTitle
            };
        }
    }
}