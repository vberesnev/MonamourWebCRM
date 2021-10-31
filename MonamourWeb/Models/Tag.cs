using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MonamourWeb.Models
{
    public abstract class Tag
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Тэг")]
        public string Title { get; set; }

        [DisplayName("Короткий тег")]
        public string ShortTitle { get; set; }

        [DisplayName("Цвет")]
        public string Color { get; set; }
    }
}