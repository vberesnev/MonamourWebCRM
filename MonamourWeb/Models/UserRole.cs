using System;

namespace MonamourWeb.Models
{
    public class UserRole : ICloneable
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public object Clone()
        {
            return new UserRole()
            {
                Id = this.Id,
                Title = this.Title
            };
        }
    }
}