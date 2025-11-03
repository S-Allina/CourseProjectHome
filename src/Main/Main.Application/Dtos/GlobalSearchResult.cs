using Main.Domain.enums.inventory;

namespace Main.Application.Dtos
{
    public class GlobalSearchResult
    {
        public string SearchTerm { get; set; } = string.Empty;
        public List<InventorySearchResult> Inventories { get; set; } = new();
        public List<ItemFieldSearchResult> ItemFields { get; set; } = new();
        public List<UserSearchResult> Users { get; set; } = new();
        public int TotalResults => Inventories.Count + ItemFields.Count + Users.Count;
    }

    public class InventorySearchResult
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int ItemsCount { get; set; }
    }

    public class ItemFieldSearchResult
    {
        public int ItemId { get; set; }
        public string ItemCustomId { get; set; } = string.Empty;
        public int InventoryId { get; set; }
        public string InventoryName { get; set; } = string.Empty;
        public string FieldName { get; set; } = string.Empty;
        public string FieldValue { get; set; } = string.Empty;
        public FieldType FieldType { get; set; }
        public string Preview => FieldValue.Length > 100 ? FieldValue.Substring(0, 100) + "..." : FieldValue;
    }

    public class UserSearchResult
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    //public class QuickSearchResult
    //{
    //    public List<QuickSearchItem> Results { get; set; } = new();
    //    public string SearchTerm { get; set; }
    //}

    //public class QuickSearchItem
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //    public string Type { get; set; } // "Inventory", "Item", "User"
    //    public string AdditionalInfo { get; set; }
    //    public string Url { get; set; }
    //}
}
