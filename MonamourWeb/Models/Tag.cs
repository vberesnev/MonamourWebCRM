using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MonamourWeb.Models
{
    public abstract class Tag : ICloneable
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "У тега должно быть название")]
        [DisplayName("Тэг")]
        public string Title { get; set; }

        [Required(ErrorMessage = "У тега должно быть короткое название")]
        [DisplayName("Короткий тег")]
        public string ShortTitle { get; set; }

        [Required(ErrorMessage = "У тега должен быть цвет")]
        [DisplayName("Цвет")]
        public string Color { get; set; }

        public abstract object Clone();
    }
}