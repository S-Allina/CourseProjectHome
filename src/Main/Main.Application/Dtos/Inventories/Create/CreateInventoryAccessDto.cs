using Main.Domain.enums.Users;

namespace Main.Application.Dtos.Inventories.Create
{
    public class CreateInventoryAccessDto
    {
        public required string UserId { get; set; }
        public AccessLevel AccessLevel { get; set; }
    }
}
