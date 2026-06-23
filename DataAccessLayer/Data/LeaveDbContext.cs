using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Data
{
    public class LeaveDbContext : DbContext
    {
        public LeaveDbContext(DbContextOptions<LeaveDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<LeaveBalance> LeaveBalances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================
            // LeaveType
            // =========================
            modelBuilder.Entity<LeaveType>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(t => t.Name).IsUnique();
            });

            // =========================
            // Employee
            // =========================
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
            });

            // =========================
            // LeaveRequest relationships
            // =========================
            modelBuilder.Entity<LeaveRequest>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Reason).HasMaxLength(500);
                entity.Property(e => e.RejectionComment).HasMaxLength(500);

                entity.HasOne(l => l.Employee)
                      .WithMany(e => e.LeaveRequests)
                      .HasForeignKey(l => l.EmployeeId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(l => l.LeaveType)
                      .WithMany()
                      .HasForeignKey(l => l.LeaveTypeId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(l => new { l.EmployeeId, l.Status });
            });

            // =========================
            // LeaveBalance relationships
            // =========================
            modelBuilder.Entity<LeaveBalance>(entity =>
            {
                entity.HasKey(b => b.Id);

                entity.HasOne(b => b.Employee)
                      .WithMany()
                      .HasForeignKey(b => b.EmployeeId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(b => b.LeaveType)
                      .WithMany()
                      .HasForeignKey(b => b.LeaveTypeId)
                      .OnDelete(DeleteBehavior.Restrict);

                // One balance row per employee / leave type.
                entity.HasIndex(b => new { b.EmployeeId, b.LeaveTypeId }).IsUnique();
            });

            // =========================
            // Seed data
            // =========================
            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = 1, Name = "Angelina", HireDate = new DateTime(2025, 01, 01) },
                new Employee { Id = 2, Name = "Marcus Lee", HireDate = new DateTime(2024, 06, 15) }
            );

            modelBuilder.Entity<LeaveType>().HasData(
                new LeaveType { Id = 1, Name = "Casual Leave", DefaultDays = 10, IsAccrued = true },
                new LeaveType { Id = 2, Name = "Sick Leave", DefaultDays = 7, IsAccrued = false },
                new LeaveType { Id = 3, Name = "Annual Leave", DefaultDays = 15, IsAccrued = true, AccrualRatePerMonth = 1.25 },
                new LeaveType { Id = 4, Name = "Maternity Leave", DefaultDays = 90, IsAccrued = false }
            );

            modelBuilder.Entity<LeaveBalance>().HasData(
                // Employee 1
                new LeaveBalance { Id = 1, EmployeeId = 1, LeaveTypeId = 1, Balance = 10 },
                new LeaveBalance { Id = 2, EmployeeId = 1, LeaveTypeId = 2, Balance = 7 },
                new LeaveBalance { Id = 3, EmployeeId = 1, LeaveTypeId = 3, Balance = 15 },
                new LeaveBalance { Id = 4, EmployeeId = 1, LeaveTypeId = 4, Balance = 90 },
                // Employee 2
                new LeaveBalance { Id = 5, EmployeeId = 2, LeaveTypeId = 1, Balance = 8 },
                new LeaveBalance { Id = 6, EmployeeId = 2, LeaveTypeId = 2, Balance = 7 },
                new LeaveBalance { Id = 7, EmployeeId = 2, LeaveTypeId = 3, Balance = 12 },
                new LeaveBalance { Id = 8, EmployeeId = 2, LeaveTypeId = 4, Balance = 90 }
            );
        }
    }
}
