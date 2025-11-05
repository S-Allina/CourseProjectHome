using Identity.Domain.Enums;
using System.Text.Json.Serialization;

namespace Identity.Application.Dto
{
    public class UserDto
    {
        public required string Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Role { get; set; } = "User";
        public required string Email { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Statuses Status { get; set; } = Statuses.Unverify;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Theme Theme { get; set; } = Theme.Light;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Language Language { get; set; } = Language.English;

        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        
    }
}
