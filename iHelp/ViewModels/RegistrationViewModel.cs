using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace iHelp.ViewModels
{
    public class RegisterViewModel
    {
        public RegisterViewModel(string name, string surname, string email, string password, string repeatPassword, string city, DateTime birthdayDate)
        {
            Name = name;
            Surname = surname;
            Email = email;
            Password = password;
            RepeatPassword = repeatPassword;
            City = city;
            BirthdayDate = birthdayDate;
        }

        [Required(ErrorMessage = "Имя является обязательным для ввода")]
        [StringLength(48, ErrorMessage = "Длина имени не должна превышать 24 символа")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Фамилия является обязательной для ввода")]
        [StringLength(48, ErrorMessage = "Длина фамилии не должна превышать 48 символов")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Электронная почта является обязательной")]
        [StringLength(248, ErrorMessage = "Длина электронной почты не должна превышать 248 символов")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Введите корректный адрес электронной почты")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль является обязательным для ввода")]
        [StringLength(24, MinimumLength = 6, ErrorMessage = "Пароль должен быть от 6 до 24 символов")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [JsonIgnore]
        [Required(ErrorMessage = "Повторите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string RepeatPassword { get; set; }

        [Required(ErrorMessage = "Город является обязательным для выбора")]
        public string City { get; set; }

        [Required(ErrorMessage = "Выберите дату рождения")]
        public DateTime BirthdayDate { get; set; }
    }
}
