using System.ComponentModel.DataAnnotations;

namespace NotebookShop.Models.Database
{
    public class aboutproject
    {
        [Display(Name = "Id")]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [StringLength(100)]
        [Display(Name = "Заголовок")]
        public string Header { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [Display(Name = "Текст")]
        public string Text { get; set; }
    }
}