using Identity.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace Identity.Domain.Entity
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Statuses Status { get; set; } = Statuses.Unverify;

        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
