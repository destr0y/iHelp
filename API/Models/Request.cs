using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Request
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Заголовок является обязательным")]
        [StringLength(48, ErrorMessage = "Заголовок должен быть не длиннее 48 символов")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Описание запроса является обязательным")]
        [StringLength(2048, ErrorMessage = "Запрос не должен превышать 2048 символов")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Вы не выбрали категорию запроса")]
        public RequestCategory Category { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [InverseProperty("Requests")]
        public Account Author { get; set; }

        [InverseProperty("PerformedRequests")]
        public Account Performer { get; set; }

        public bool IsCompleted { get; set; }

        public Review Review { get; set; }
    }

    public enum RequestCategory
    {
        Transport = 1,
        Shop,
        Pet,
        Medical
    }
}
