using Kish_AndreiCezar_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace Kish_AndreiCezar_Project.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Manufacturer> Manufacturers => Set<Manufacturer>();
    public DbSet<CarModel> CarModels => Set<CarModel>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Mechanic> Mechanics => Set<Mechanic>();
    public DbSet<ServiceTicket> ServiceTickets => Set<ServiceTicket>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<ServiceTicket>()
            .Property(p => p.IntakeDate)
            .HasConversion(
                v => v.ToDateTime(TimeOnly.MinValue),
                v => DateOnly.FromDateTime(v));

        modelBuilder.Entity<Manufacturer>().HasData(
            new Manufacturer { Id = 1, Name = "Toyota", Country = "Japonia" },
            new Manufacturer { Id = 2, Name = "BMW", Country = "Germania" },
            new Manufacturer { Id = 3, Name = "Dacia", Country = "Romania" }
        );

        modelBuilder.Entity<CarModel>().HasData(
            new CarModel { Id = 1, Name = "Car", YearFrom = 2020, VehicleType = "Car", ManufacturerId = 1 },
            new CarModel { Id = 2, Name = "SUV", YearFrom = 2019, VehicleType = "SUV", ManufacturerId = 2 },
            new CarModel { Id = 3, Name = "Truck", YearFrom = 2018, VehicleType = "Truck", ManufacturerId = 3 },
            new CarModel { Id = 4, Name = "Van", YearFrom = 2021, VehicleType = "Truck", ManufacturerId = 1 },
            new CarModel { Id = 5, Name = "Bus", YearFrom = 2017, VehicleType = "Bus", ManufacturerId = 2 },
            new CarModel { Id = 6, Name = "Motorcycle", YearFrom = 2022, VehicleType = "Motorcycle", ManufacturerId = 3 }
        );

        modelBuilder.Entity<Customer>().HasData(
            new Customer { Id = 1, FullName = "Ana Pop", Email = "ana.pop@example.com", Phone = "0711111111" },
            new Customer { Id = 2, FullName = "Mihai Ionescu", Email = "mihai.ionescu@example.com", Phone = "0722222222" }
        );

        modelBuilder.Entity<Mechanic>().HasData(
            new Mechanic { Id = 1, FullName = "Ioan Marinescu", Specialty = "Diagnostica", YearsExperience = 8 },
            new Mechanic { Id = 2, FullName = "Sorin Enache", Specialty = "Motor", YearsExperience = 12 }
        );

        modelBuilder.Entity<ServiceTicket>().HasData(
            new ServiceTicket
            {
                Id = 1,
                CarModelId = 1, // Car
                CustomerId = 1,
                MechanicId = 1,
                IntakeDate = new DateOnly(2024, 12, 1),
                Complaint = "Verificare anuala si schimb ulei",
                MileageKm = 71128,
                Status = "Programat",
                EstimatedHours = 3.5f,
                EstimatedCost = 900
            },
            new ServiceTicket
            {
                Id = 2,
                CarModelId = 2, // SUV
                CustomerId = 2,
                MechanicId = 2,
                IntakeDate = new DateOnly(2024, 12, 5),
                Complaint = "Vibratii la franare",
                MileageKm = 59673,
                Status = "In lucru",
                EstimatedHours = 2.5f,
                EstimatedCost = 650
            });
    }
}

