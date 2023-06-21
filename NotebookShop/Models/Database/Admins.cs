using System.ComponentModel.DataAnnotations;

namespace NotebookShop.Models.Database
{
    public class admins
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [StringLength(50)]
        [DataType(DataType.EmailAddress, ErrorMessage = "Неправильный формат Email!")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [StringLength(30)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
    }
}