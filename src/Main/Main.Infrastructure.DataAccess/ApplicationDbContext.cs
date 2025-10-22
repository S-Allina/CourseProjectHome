using Main.Domain.entities.Comments;
using Main.Domain.entities.common;
using Main.Domain.entities.inventory;
using Main.Domain.entities.item;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Main.Infrastructure.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // DbSets
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
            // CHANGE THIS: Restrict -> Cascade для InventoryField -> ItemFieldValues
            modelBuilder.Entity<ItemFieldValue>()
                .HasOne(iv => iv.InventoryField)
                .WithMany()
                .HasForeignKey(iv => iv.InventoryFieldId)
                .OnDelete(DeleteBehavior.Cascade); // Изменено с Restrict на Cascade

            // ОСТАВИТЬ: Item -> ItemFieldValues как Restrict
            modelBuilder.Entity<ItemFieldValue>()
                .HasOne(iv => iv.Item)
                .WithMany(i => i.FieldValues)
                .HasForeignKey(iv => iv.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // Остальной код без изменений...
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

            modelBuilder.Entity<Like>()
                .HasKey(x => new { x.ItemId, x.UserId });

            modelBuilder.Entity<InventoryTag>()
                .HasKey(x => new { x.InventoryId, x.TagId });

            modelBuilder.Entity<InventoryAccess>()
                .HasKey(x => new { x.InventoryId, x.UserId });

            modelBuilder.Entity<Inventory>()
                .HasMany(i => i.Fields)
                .WithOne(f => f.Inventory)
                .HasForeignKey(f => f.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Inventory>()
                .HasMany(i => i.Items)
                .WithOne(item => item.Inventory)
                .HasForeignKey(item => item.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);

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
                .HasIndex(i => i.CreatedById);

            modelBuilder.Entity<Comment>()
                .HasIndex(c => c.InventoryId);

            modelBuilder.Entity<Comment>()
                .HasIndex(c => c.AuthorId);

            modelBuilder.Entity<Tag>()
                .HasIndex(t => t.Name)
                .IsUnique();

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique();

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
