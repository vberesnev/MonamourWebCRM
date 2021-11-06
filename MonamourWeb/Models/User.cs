using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MonamourWeb.Models
{
    public class User : ICloneable
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Пользователь должен иметь логин")]
        [DisplayName("Логин")]
        public string Name { get; set; }
        
        [DisplayName("Пароль")]
        public string Password { get; set; }
        
        [DisplayName("Роль")]
        public UserRole Role { get; set; }
        
        [Required(ErrorMessage = "Пользователь должен иметь роль")]
        public int RoleId { get; set; }

        [DisplayName("Заблокирован")]
        public bool Blocked { get; set; }

        public override string ToString()
        {
            return $"Пользователь: [Id]: {Id}; [Логин]: {Name}; [Id роли]: {RoleId}; [Роль]: {Role?.Title}; [Заблокирован]: {Helper.BoolToStringConverter(Blocked)}";
        }

        public object Clone()
        {
            return new User()
            {
                Id = this.Id,
                Name = this.Name,
                Password = this.Password,
                Role = this.Role?.Clone() as UserRole,
                RoleId = this.RoleId,
                Blocked = this.Blocked
            };
        }
    }
}