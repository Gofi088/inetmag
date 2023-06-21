using System.ComponentModel.DataAnnotations;

namespace NotebookShop.Models.Database
{
    public class basket
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [StringLength(10)]
        [Display(Name = "Куки юзера")]
        public string UserKey { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [StringLength(10)]
        [Display(Name = "Таблица (system)")]
        public string TableName { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [Display(Name = "Id")]
        public int ModelId { get; set; }
    }
}