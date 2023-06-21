using System.ComponentModel.DataAnnotations;

namespace NotebookShop.Models.Database
{
    public class videocards
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
        [StringLength(50)]
        [Display(Name = "Тактовая частота")]
        public string ClockSpeed { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [StringLength(50)]
        [Display(Name = "Память")]
        public string Memory { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [StringLength(50)]
        [Display(Name = "Системная шина")]
        public string SystemBus { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [StringLength(50)]
        [Display(Name = "DirectX")]
        public string DirectX { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [StringLength(50)]
        [Display(Name = "Интерфейс")]
        public string Interface { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [StringLength(255)]
        [Display(Name = "Превью картинка")]
        public string Photo { get; set; }
    }
}