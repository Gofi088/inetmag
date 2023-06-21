using System.ComponentModel.DataAnnotations;

namespace NotebookShop.Models.Database
{
    public class screens
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [StringLength(100)]
        [Display(Name = "Модель")]
        public string Model { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [StringLength(255)]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [Display(Name = "Диагональ")]
        public int Diagonal { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [StringLength(50)]
        [Display(Name = "Тип экрана")]
        public string ScreenType { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [StringLength(50)]
        [Display(Name = "Цена")]
        public string Cost { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [StringLength(255)]
        [Display(Name = "Превью картинка")]
        public string Photo { get; set; }
    }
}