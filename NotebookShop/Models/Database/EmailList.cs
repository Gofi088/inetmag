using System.ComponentModel.DataAnnotations;

namespace NotebookShop.Models.Database
{
    public class emaillist
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [StringLength(50)]
        [DataType(DataType.EmailAddress, ErrorMessage = "Неправильный формат Email!")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [StringLength(50)]
        [Display(Name = "Дата создания записи")]
        public string CreateDate { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [StringLength(50)]
        [Display(Name = "Время создания записи")]
        public string CreateTime { get; set; }
    }
}