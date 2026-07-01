using Microsoft.EntityFrameworkCore;
using RoadReady.API.Models;
using System;
using System.Security.Cryptography;
using System.Text;

namespace RoadReady.API.Data
{
    public class RoadReadyDbContext : DbContext
    {
        public RoadReadyDbContext(DbContextOptions<RoadReadyDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Car> Cars { get; set; } = null!;
        public DbSet<Reservation> Reservations { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Delete Behaviors
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Car)
                .WithMany()
                .HasForeignKey(r => r.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Reservation)
                .WithMany()
                .HasForeignKey(p => p.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(rv => rv.Car)
                .WithMany()
                .HasForeignKey(rv => rv.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(rv => rv.User)
                .WithMany()
                .HasForeignKey(rv => rv.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed Users
            var adminPasswordHash = HashPassword("Admin@123");
            var customerPasswordHash = HashPassword("Customer@123");

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FullName = "Admin User",
                    Email = "admin@roadready.com",
                    PasswordHash = adminPasswordHash,
                    Role = "Admin",
                    CreatedAt = DateTime.Parse("2026-01-01")
                },
                new User
                {
                    Id = 2,
                    FullName = "John Doe",
                    Email = "john@roadready.com",
                    PasswordHash = customerPasswordHash,
                    Role = "Customer",
                    CreatedAt = DateTime.Parse("2026-01-01")
                }
            );

            // Seed Cars
            modelBuilder.Entity<Car>().HasData(
                new Car
                {
                    Id = 1,
                    Make = "Tesla",
                    Model = "Model S",
                    Year = 2024,
                    Type = "Electric",
                    Transmission = "Automatic",
                    FuelType = "Electric",
                    Seats = 5,
                    PricePerDay = 120.00m,
                    IsAvailable = true,
                    ImageUrl = "https://images.unsplash.com/photo-1614162692292-7ac56d7f7f1e?auto=format&fit=crop&q=80&w=600",
                    Description = "High-performance electric luxury sedan with autopilot capabilities, long-range battery, and dual motor AWD."
                },
                new Car
                {
                    Id = 2,
                    Make = "BMW",
                    Model = "M4 Coupé",
                    Year = 2023,
                    Type = "Luxury",
                    Transmission = "Automatic",
                    FuelType = "Petrol",
                    Seats = 4,
                    PricePerDay = 150.00m,
                    IsAvailable = true,
                    ImageUrl = "https://images.unsplash.com/photo-1617814076367-b759c7d7e738?auto=format&fit=crop&q=80&w=600",
                    Description = "Sporty luxury coupé with twin-turbo power, aggressive styling, carbon-fiber roof, and precision handling."
                },
                new Car
                {
                    Id = 3,
                    Make = "Ford",
                    Model = "Mustang GT",
                    Year = 2022,
                    Type = "Sports",
                    Transmission = "Manual",
                    FuelType = "Petrol",
                    Seats = 4,
                    PricePerDay = 95.00m,
                    IsAvailable = true,
                    ImageUrl = "https://images.unsplash.com/photo-1612462551853-fc8838f10a68?auto=format&fit=crop&q=80&w=600",
                    Description = "Classic American muscle car featuring a roaring 5.0L V8 engine, six-speed manual gearbox, and raw exhaust sound."
                },
                new Car
                {
                    Id = 4,
                    Make = "Toyota",
                    Model = "RAV4 Hybrid",
                    Year = 2023,
                    Type = "SUV",
                    Transmission = "Automatic",
                    FuelType = "Hybrid",
                    Seats = 5,
                    PricePerDay = 65.00m,
                    IsAvailable = true,
                    ImageUrl = "https://images.unsplash.com/photo-1621007947382-bb3c3994e3fb?auto=format&fit=crop&q=80&w=600",
                    Description = "Highly efficient compact crossover SUV featuring electronic all-wheel drive, spacious cargo area, and top safety features."
                },
                new Car
                {
                    Id = 5,
                    Make = "Honda",
                    Model = "Civic Touring",
                    Year = 2023,
                    Type = "Sedan",
                    Transmission = "Automatic",
                    FuelType = "Petrol",
                    Seats = 5,
                    PricePerDay = 45.00m,
                    IsAvailable = true,
                    ImageUrl = "https://images.unsplash.com/photo-1533473359331-0135ef1b58bf?auto=format&fit=crop&q=80&w=600",
                    Description = "The ultimate reliable daily sedan, packed with modern tech options, comfortable leather seats, and great fuel efficiency."
                },
                new Car
                {
                    Id = 6,
                    Make = "Hyundai",
                    Model = "Ioniq 5",
                    Year = 2024,
                    Type = "Electric",
                    Transmission = "Automatic",
                    FuelType = "Electric",
                    Seats = 5,
                    PricePerDay = 80.00m,
                    IsAvailable = true,
                    ImageUrl = "https://images.unsplash.com/photo-1668472506803-b1d5bf590515?auto=format&fit=crop&q=80&w=600",
                    Description = "Retro-futuristic electric utility vehicle featuring ultra-fast 800V charging, premium interior comfort, and head-turning styling."
                },
                new Car
                {
                    Id = 7,
                    Make = "Chevrolet",
                    Model = "Tahoe LT",
                    Year = 2022,
                    Type = "SUV",
                    Transmission = "Automatic",
                    FuelType = "Petrol",
                    Seats = 7,
                    PricePerDay = 110.00m,
                    IsAvailable = true,
                    ImageUrl = "https://images.unsplash.com/photo-1533473359331-0135ef1b58bf?auto=format&fit=crop&q=80&w=600",
                    Description = "Full-size SUV featuring three rows of seats, robust V8 utility engine, premium sound system, and generous luggage capacity."
                }
            );
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
