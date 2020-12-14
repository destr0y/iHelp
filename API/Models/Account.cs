using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace API.Models
{
    public class Account
    {
        public int Id { get; set; }

        [Required]
        [StringLength(48)]
        public string Name { get; set; }

        [Required]
        [StringLength(48)]
        public string Surname { get; set; }

        [Required]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [JsonIgnore]
        [StringLength(24)]
        public string Password { get; set; }

        [Required]
        public string City { get; set; }

        [JsonIgnore]
        public List<Request> Requests { get; set; }

        public int RequestsCount => Requests?.Count ?? 0;

        [JsonIgnore]
        public List<Request> PerformedRequests { get; set; }

        public int PerformedCount => PerformedRequests?.Count ?? 0;

        [Required]
        public DateTime BirthdayDate { get; set; }

        public static explicit operator Account(RegisterModel model)
        {
            return new Account
            {
                Name = model.Name,
                Surname = model.Surname,
                Email = model.Email,
                Password = model.Password,
                City = model.City,
                BirthdayDate = model.BirthdayDate
            };
        }
    }
}
