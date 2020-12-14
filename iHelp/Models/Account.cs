using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace iHelp.Models
{
    public class Account
    {
        [JsonIgnore]
        public  int Id { get; set; }

        [Required(ErrorMessage = "Имя является обязательным полем")]
        [StringLength(48, ErrorMessage = "Длина имени не должна превышать 48 символов")]
        public string Name { get; set; }

        [Required]
        [StringLength(48)]
        public string Surname { get; set; }

        [Required]
        [StringLength(255)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [StringLength(24)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime BirthdayDate { get; set; }

        public List<Review> Reviews { get; set; }

        public int RequestsCount { get; set; }

        public int PerformedCount { get; set; }
    }
}
