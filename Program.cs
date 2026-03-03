using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;


public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }

    public List<Order> Orders { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }

    public List<Order> Orders { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
}

public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public List<Product> Products { get; set; } = new();
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }

    public List<Product> Products { get; set; } = new();
}

public class Review
{
    public int Id { get; set; }
    public string Comment { get; set; }
    public int Rating { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; }
}
public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer(
            "Server=(localdb)\\mssqllocaldb;Database=TrainingDb;Trusted_Connection=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .HasMany(p => p.Categories)
            .WithMany(c => c.Products);

        modelBuilder.Entity<Order>()
            .HasMany(o => o.Products)
            .WithMany(p => p.Orders);
    }
}
public class Repository<T> where T : class
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public void Add(T entity)
    {
        _dbSet.Add(entity);
        _context.SaveChanges();
    }

    public List<T> GetAll()
    {
        return _dbSet.ToList();
    }

    public T GetById(int id)
    {
        return _dbSet.Find(id);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var entity = _dbSet.Find(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }
    }
}

class Program
{
    static void Main()
    {
        using var context = new AppDbContext();

        var userRepo = new Repository<User>(context);
        var productRepo = new Repository<Product>(context);
        var categoryRepo = new Repository<Category>(context);
        var orderRepo = new Repository<Order>(context);
        var reviewRepo = new Repository<Review>(context);

        // CREATE
        var user = new User { Name = "Іван", Email = "ivan@email.com" };
        userRepo.Add(user);

        var category = new Category { Name = "Електроніка" };
        categoryRepo.Add(category);

        var product = new Product { Name = "Ноутбук", Price = 30000 };
        product.Categories.Add(category);
        productRepo.Add(product);

        var order = new Order
        {
            OrderDate = DateTime.Now,
            UserId = user.Id
        };
        order.Products.Add(product);
        orderRepo.Add(order);

        var review = new Review
        {
            Comment = "Дуже хороший товар",
            Rating = 5,
            UserId = user.Id,
            ProductId = product.Id
        };
        reviewRepo.Add(review);

        // READ
        var users = userRepo.GetAll();
        Console.WriteLine("Користувачі:");
        foreach (var u in users)
        {
            Console.WriteLine($"{u.Id} {u.Name} {u.Email}");
        }

        // UPDATE
        user.Name = "Іван Петренко";
        userRepo.Update(user);

        // DELETE (приклад)
        // reviewRepo.Delete(review.Id);

        Console.WriteLine("Готово!");
    }
}
