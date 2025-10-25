using Main.Domain.enums.Users;

namespace Main.Application.Dtos.Inventories.Create
{
    public class CreateInventoryAccessDto
    {
        public string UserId { get; set; }
        public AccessLevel AccessLevel { get; set; }
    }
}
