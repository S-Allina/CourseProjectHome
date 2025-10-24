using Main.Domain.entities.Comments;
using Main.Domain.entities.common;
using Main.Domain.entities.inventory;
using Main.Domain.entities.item;
using Microsoft.EntityFrameworkCore;

namespace Main.Infrastructure.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // DbSets
        public DbSet<User> Users { get; set; } // ← Новая таблица
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<InventoryField> InventoryFields { get; set; }
        public DbSet<InventoryAccess> InventoryAccess { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemFieldValue> ItemFieldValues { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> ItemLikes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<InventoryTag> InventoryTags { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ===== КОНФИГУРАЦИЯ USER =====
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                // Основные индексы для поиска
                entity.HasIndex(u => u.Email)
                      .IsUnique()
                      .HasDatabaseName("IX_Users_Email");

                entity.HasIndex(u => new { u.FirstName, u.LastName })
                      .HasDatabaseName("IX_Users_FirstName_LastName");

                entity.HasIndex(u => u.LastName)
                      .HasDatabaseName("IX_Users_LastName");

                // Индекс для полнотекстового поиска (если БД поддерживает)
                entity.HasIndex(u => new { u.FirstName, u.LastName, u.Email })
                      .HasDatabaseName("IX_Users_Search")
                      .IsClustered(false);

                // Ограничения длины
                entity.Property(u => u.Id)
                      .HasMaxLength(450); // Стандартная длина для ID пользователей

                entity.Property(u => u.FirstName)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(u => u.LastName)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(u => u.Email)
                      .HasMaxLength(256)
                      .IsRequired();
            });

            // ===== ОБНОВЛЕННЫЕ СВЯЗИ ДЛЯ СУЩЕСТВУЮЩИХ СУЩНОСТЕЙ =====

            // InventoryAccess -> User связи
            modelBuilder.Entity<InventoryAccess>()
                .HasOne(ia => ia.User)
                .WithMany(u => u.InventoryAccesses)
                .HasForeignKey(ia => ia.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InventoryAccess>()
                .HasOne(ia => ia.GrantedBy)
                .WithMany()
                .HasForeignKey(ia => ia.GrantedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Item -> User связь
            modelBuilder.Entity<Item>()
                .HasOne(i => i.CreatedBy)
                .WithMany(u => u.CreatedItems)
                .HasForeignKey(i => i.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Comment -> User связь
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Author)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Like -> User связь
            modelBuilder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== СУЩЕСТВУЮЩАЯ КОНФИГУРАЦИЯ (с небольшими обновлениями) =====

            // ItemFieldValue конфигурация
            modelBuilder.Entity<ItemFieldValue>()
                .HasOne(iv => iv.InventoryField)
                .WithMany()
                .HasForeignKey(iv => iv.InventoryFieldId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ItemFieldValue>()
                .HasOne(iv => iv.Item)
                .WithMany(i => i.FieldValues)
                .HasForeignKey(iv => iv.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // Inventory конфигурация
            modelBuilder.Entity<Inventory>()
                .HasMany(i => i.Items)
                .WithOne(item => item.Inventory)
                .HasForeignKey(item => item.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.Category)
                .WithMany()
                .HasForeignKey(i => i.CategoryId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Inventory>()
                .HasMany(i => i.Fields)
                .WithOne(f => f.Inventory)
                .HasForeignKey(f => f.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ключи для связных таблиц
            modelBuilder.Entity<Like>()
                .HasKey(x => new { x.ItemId, x.UserId });

            modelBuilder.Entity<InventoryTag>()
                .HasKey(x => new { x.InventoryId, x.TagId });

            modelBuilder.Entity<InventoryAccess>()
                .HasKey(x => new { x.InventoryId, x.UserId });

            // Индексы для производительности
            modelBuilder.Entity<Item>()
                .HasIndex(i => new { i.InventoryId, i.CustomId })
                .IsUnique()
                .HasFilter("[CustomId] IS NOT NULL");

            modelBuilder.Entity<Inventory>()
                .HasIndex(i => i.OwnerId);

            modelBuilder.Entity<Inventory>()
                .HasIndex(i => i.IsPublic);

            modelBuilder.Entity<Item>()
                .HasIndex(i => i.InventoryId);

            modelBuilder.Entity<Item>()
                .HasIndex(i => i.CreatedById); // Теперь это ссылка на Users

            modelBuilder.Entity<Comment>()
                .HasIndex(c => c.InventoryId);

            modelBuilder.Entity<Comment>()
                .HasIndex(c => c.AuthorId); // Теперь это ссылка на Users

            modelBuilder.Entity<Tag>()
                .HasIndex(t => t.Name)
                .IsUnique();

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique();

            // RowVersion для оптимистичной блокировки
            modelBuilder.Entity<Inventory>()
                .Property(i => i.Version)
                .IsRowVersion();

            modelBuilder.Entity<Item>()
                .Property(i => i.Version)
                .IsRowVersion();

            modelBuilder.Entity<InventoryField>()
                .Property(f => f.FieldType)
                .HasConversion<int>();
        }
    }
}
