using Main.Domain.enums.inventory;

namespace Main.Domain.entities
{
    public class GlobalSearchResult
    {
        public string SearchTerm { get; set; }
        public List<InventorySearchResult> Inventories { get; set; } = new();
        public List<ItemFieldSearchResult> ItemFields { get; set; } = new();
        public List<UserSearchResult> Users { get; set; } = new();
        public bool IsTagSearch { get; set; }
        public int TotalResults => Inventories.Count + ItemFields.Count + Users.Count;
    }

    public class InventorySearchResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ItemsCount { get; set; }
    }

    public class ItemFieldSearchResult
    {
        public int ItemId { get; set; }
        public string ItemCustomId { get; set; }
        public int InventoryId { get; set; }
        public string InventoryName { get; set; }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public FieldType FieldType { get; set; }
        public string Preview => FieldValue.Length > 100 ? FieldValue.Substring(0, 100) + "..." : FieldValue;
    }

    public class QuickSearchResult
    {
        public List<QuickSearchItem> Results { get; set; } = new();
        public string SearchTerm { get; set; }
    }

    public class QuickSearchItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; } // "Inventory", "Item", "User"
        public string AdditionalInfo { get; set; }
        public string Url { get; set; }
    }
}
