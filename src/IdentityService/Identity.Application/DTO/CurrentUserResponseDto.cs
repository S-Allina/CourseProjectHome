using Identity.Domain.Enums;
using System.Text.Json.Serialization;

namespace Identity.Application.Dto
{
    public class CurrentUserDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool EmailConfirmed { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Statuses Status { get; set; } = Statuses.Unverify;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Theme Theme { get; set; } = Theme.Light;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Language Language { get; set; } = Language.English;

        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string Role { get; set; } = "User";
        public string Email { get; set; }
    }
}
