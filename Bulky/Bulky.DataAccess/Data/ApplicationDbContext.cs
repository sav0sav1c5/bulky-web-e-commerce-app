using Bulky.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        // DB Set
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        // Table Seed
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
                new Category { Id = 2, Name = "SciFi", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Horor", DisplayOrder = 3 },
                new Category { Id = 4, Name = "History", DisplayOrder = 4 }
                );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Title = "The Subtle Art of Not Giving a F*ck",
                    Description = "A counterintuitive approach to living a good life. This book challenges the conventional wisdom about positivity and happiness, arguing that accepting our limitations and embracing our fears leads to more meaningful lives.",
                    ISBN = "978-0062457714",
                    Author = "Mark Manson",
                    ListPrice = 26.99,
                    Price = 24.99,
                    Price50 = 22.99,
                    Price100 = 19.99,
                    CategoryId = 1,
                    ImageURL = "" 
                },
                new Product
                {
                    Id = 2,
                    Title = "How to Win Friends and Influence People",
                    Description = "This timeless bestseller has helped countless people climb the ladder of success in their business and personal lives. Learn the six ways to make people like you, the twelve ways to win people to your way of thinking, and the nine ways to change people without arousing resentment.",
                    ISBN = "978-0671027032",
                    Author = "Dale Carnegie",
                    ListPrice = 19.99,
                    Price = 17.99,
                    Price50 = 15.99,
                    Price100 = 13.99,
                    CategoryId = 3, 
                    ImageURL = ""
                },
                new Product
                {
                    Id = 3,
                    Title = "Onyx & Ivory",
                    Description = "A sweeping fantasy about a kingdom divided by magic, where a girl with forbidden magic must unite with a disgraced royal in order to save their world from destruction.",
                    ISBN = "978-0062652668",
                    Author = "Rebecca Yarros",
                    ListPrice = 18.99,
                    Price = 16.99,
                    Price50 = 15.50,
                    Price100 = 14.50,
                    CategoryId = 2,
                    ImageURL = ""
                },
                new Product
                {
                    Id = 4,
                    Title = "Read People Like a Book",
                    Description = "How to analyze, understand, and predict people's emotions, thoughts, intentions, and behaviors. This practical guide provides insights into human psychology and teaches techniques for better understanding others.",
                    ISBN = "978-1647433727",
                    Author = "Patrick King",
                    ListPrice = 16.99,
                    Price = 15.99,
                    Price50 = 14.50,
                    Price100 = 12.99,
                    CategoryId = 3,
                    ImageURL = "/images/products/read-people-like-a-book.jpg"
                },
                new Product
                {
                    Id = 5,
                    Title = "Rich Dad Poor Dad",
                    Description = "What the rich teach their kids about money that the poor and middle class do not. This personal finance classic challenges conventional views on work, saving, investing and building wealth.",
                    ISBN = "978-1612680194",
                    Author = "Robert T. Kiyosaki",
                    ListPrice = 17.95,
                    Price = 15.95,
                    Price50 = 14.50,
                    Price100 = 12.99,
                    CategoryId = 2, 
                    ImageURL = ""
                },
                new Product
                {
                    Id = 6,
                    Title = "The Wheel of Time: The Eye of the World",
                    Description = "The first book in Robert Jordan's epic fantasy series. When their village is attacked by terrifying creatures, three young men begin a perilous journey that will change their lives forever.",
                    ISBN = "978-1250251466",
                    Author = "Robert Jordan",
                    ListPrice = 22.99,
                    Price = 19.99,
                    Price50 = 18.50,
                    Price100 = 16.99,
                    CategoryId = 2,
                    ImageURL = ""
                },
                new Product
                {
                    Id = 7,
                    Title = "The Psychology of Money",
                    Description = "Timeless lessons on wealth, greed, and happiness. This book explores how your personal experiences with money shape your behavior more than logic or facts, and how to make better financial decisions.",
                    ISBN = "978-0857197689",
                    Author = "Morgan Housel",
                    ListPrice = 19.99,
                    Price = 17.99,
                    Price50 = 16.50,
                    Price100 = 14.99,
                    CategoryId = 4,
                    ImageURL = ""
                },
                new Product
                {
                    Id = 8,
                    Title = "Yoshitomo Nara Art Book",
                    Description = "A delightful collection showcasing the whimsical and distinctive art of Yoshitomo Nara, featuring his iconic wide-eyed children and animals that blend innocence with defiance.",
                    ISBN = "978-0714857466",
                    Author = "Yoshitomo Nara",
                    ListPrice = 45.00,
                    Price = 42.99,
                    Price50 = 40.50,
                    Price100 = 38.99,
                    CategoryId = 2,
                    ImageURL = ""
                },
                new Product
                {
                    Id = 9,
                    Title = "Fortune of Time",
                    Author = "Billy Spark",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "SWD9999001",
                    ListPrice = 99,
                    Price = 90,
                    Price50 = 85,
                    Price100 = 80,
                    CategoryId = 1,
                    ImageURL = ""
                },
                new Product
                {
                    Id = 10,
                    Title = "Dark Skies",
                    Author = "Nancy Hoover",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "CAW777777701",
                    ListPrice = 40,
                    Price = 30,
                    Price50 = 25,
                    Price100 = 20,
                    CategoryId = 2,
                    ImageURL = ""
                },
                new Product
                {
                    Id = 11,
                    Title = "Vanish in the Sunset",
                    Author = "Julian Button",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "RITO5555501",
                    ListPrice = 55,
                    Price = 50,
                    Price50 = 40,
                    Price100 = 35,
                    CategoryId = 2,
                    ImageURL = ""
                },
                new Product
                {
                    Id = 12,
                    Title = "Cotton Candy",
                    Author = "Abby Muscles",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "WS3333333301",
                    ListPrice = 70,
                    Price = 65,
                    Price50 = 60,
                    Price100 = 55,
                    CategoryId = 3,
                    ImageURL = ""
                },
                new Product
                {
                    Id = 13,
                    Title = "Rock in the Ocean",
                    Author = "Ron Parker",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "SOTJ1111111101",
                    ListPrice = 30,
                    Price = 27,
                    Price50 = 25,
                    Price100 = 20,
                    CategoryId = 4,
                    ImageURL = ""
                },
                new Product
                {
                    Id = 14,
                    Title = "Leaves and Wonders",
                    Author = "Laura Phantom",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "FOT000000001",
                    ListPrice = 25,
                    Price = 23,
                    Price50 = 22,
                    Price100 = 20,
                    CategoryId = 4,
                    ImageURL = ""
                }
            );

            modelBuilder.Entity<Company>().HasData(
                new Company
                {
                    Id = 1,
                    Name = "NIS Gasprom Neft",
                    StreetAddress = "Narodnog fronta 12",
                    City = "Novi Sad",
                    State = "Serbia",
                    PhoneNumber = "021-481-1111",
                    PostalCode = "21000"
                }, 
                new Company
                {
                    Id = 2,
                    Name = "Telekom Srbija",
                    StreetAddress = "Takovska 1",
                    City = "Belgrade",
                    State = "Serbia",
                    PhoneNumber = "011-123-4567",
                    PostalCode = "11000"
                },
                new Company
                {
                    Id = 3,
                    Name = "Hemofarm",
                    StreetAddress = "Beogradski put bb",
                    City = "Vrsac",
                    State = "Serbia",
                    PhoneNumber = "021-481-1111",
                    PostalCode = "26300"
                } 
            );
        }
    }
}
