using System.ComponentModel.DataAnnotations;

namespace NotebookShop.Models.Database
{
    public class orders
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [StringLength(50)]
        [Display(Name = "Дата заказа")]
        public string DateOrder { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [StringLength(100)]
        [Display(Name = "ФИО")]
        public string Initials { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [StringLength(30)]
        [Display(Name = "Адрес")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [StringLength(50)]
        [Display(Name = "Итоговая стоимость")]
        public string Cost { get; set; }
    }
}