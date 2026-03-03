using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace _27._01._2026.Models;

public partial class MyDbContext : DbContext
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost; Database=ShopDb;Trusted_Connection=True; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasMany(d => d.Products).WithMany(p => p.Categories)
                .UsingEntity<Dictionary<string, object>>(
                    "CategoryProduct",
                    r => r.HasOne<Product>().WithMany().HasForeignKey("ProductsId"),
                    l => l.HasOne<Category>().WithMany().HasForeignKey("CategoriesId"),
                    j =>
                    {
                        j.HasKey("CategoriesId", "ProductsId");
                        j.ToTable("CategoryProduct");
                        j.HasIndex(new[] { "ProductsId" }, "IX_CategoryProduct_ProductsId");
                    });
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_Orders_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.Orders).HasForeignKey(d => d.UserId);

            entity.HasMany(d => d.Products).WithMany(p => p.Orders)
                .UsingEntity<Dictionary<string, object>>(
                    "OrderProduct",
                    r => r.HasOne<Product>().WithMany().HasForeignKey("ProductsId"),
                    l => l.HasOne<Order>().WithMany().HasForeignKey("OrdersId"),
                    j =>
                    {
                        j.HasKey("OrdersId", "ProductsId");
                        j.ToTable("OrderProduct");
                        j.HasIndex(new[] { "ProductsId" }, "IX_OrderProduct_ProductsId");
                    });
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasIndex(e => e.ProductId, "IX_Reviews_ProductId");

            entity.HasIndex(e => e.UserId, "IX_Reviews_UserId");

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews).HasForeignKey(d => d.ProductId);

            entity.HasOne(d => d.User).WithMany(p => p.Reviews).HasForeignKey(d => d.UserId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
