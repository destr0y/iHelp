using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class RegisterModel
    {
        [Required]
        [StringLength(48)]
        public string Name { get; set; }

        [Required]
        [StringLength(48)]
        public string Surname { get; set; }

        [Required]
        [StringLength(248)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [StringLength(24)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public DateTime BirthdayDate { get; set; }
    }
}
