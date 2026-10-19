using Microsoft.EntityFrameworkCore;
using PeopleCore.Api.Models;
namespace PeopleCore.Api.Data;
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>(); public DbSet<Department> Departments => Set<Department>(); public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>(); public DbSet<AppUser> Users => Set<AppUser>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<Employee>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<Department>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<Employee>().HasOne(x => x.Department).WithMany(x => x.Employees).HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<LeaveRequest>().HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LeaveRequest>().Property(x => x.Status).HasConversion<string>();
    }
}
