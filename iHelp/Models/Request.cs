using System;
using System.ComponentModel.DataAnnotations;
using iHelp.Models;

namespace iHelp.Models
{
    public class Request
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите заголовок")]
        [StringLength(50, ErrorMessage = "Заголовок не должен превышать 50 символов")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Введите текст запроса")]
        [StringLength(1024, ErrorMessage = "Запрос не должен превышать 1024 символа")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Выберите категорию запроса")]
        public RequestCategory Category { get; set; }

        [Required(ErrorMessage = "Введите адрес для запроса")]
        [StringLength(255, ErrorMessage = "Адрес не должен превышать 255 символв")]
        public string Location { get; set; }

        public DateTime CreationDate { get; set; }
        public Account Author { get; set; }
        public Account Performer { get; set; }
        public bool IsCompleted { get; set; }
        public Review Review { get; set; }

        public Request(string title, string description, RequestCategory category, string location)
        {
            Title = title;
            Description = description;
            Category = category;
            Location = location;
            CreationDate = DateTime.Now;
        }
    }

    public enum RequestCategory
    {
        Transport = 1,
        Shop,
        Pet,
        Medical
    }
}
