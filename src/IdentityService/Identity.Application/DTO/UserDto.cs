using Identity.Domain.Enums;
using System.Text.Json.Serialization;

namespace Identity.Application.Dto
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; } = false;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Statuses? Status { get; set; } = Statuses.Unverify;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Theme Theme { get; set; } = Theme.Light;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Language Language { get; set; } = Language.English;

        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string Role { get; set; } = "User";

    }
}
