using Microsoft.EntityFrameworkCore;
using Nurse_Recording_Backend.Models;

namespace Nurse_Recording_Backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<PatientRecord> PatientRecords { get; set; }
    public DbSet<Followup> Followups { get; set; }
    public DbSet<Nurse> Nurses { get; set; }
    public DbSet<Alarm> Alarms { get; set; }
    public DbSet<Device> Devices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Alarm>()
            .HasOne(a => a.Device)
            .WithMany(d => d.Alarms)
            .HasForeignKey(a => a.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed data
        modelBuilder.Entity<Nurse>().HasData(
            new Nurse { Id = 1, Username = "aclcnurse", Password = "aclcnurse123", Email = "aclcnurse@gmail.com", IsAuthenticated = true }
        );

        base.OnModelCreating(modelBuilder);
    }
}
