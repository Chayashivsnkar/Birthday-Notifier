using System;
using System.Text.Json.Serialization;

namespace BirthdayNotifier.Models
{
    public class UserCreateDto
    {
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }

        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime? DOB { get; set; }

        public string? Email { get; set; }
    }
}
