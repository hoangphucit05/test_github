using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Login.DAL.Entities;

namespace Login.DAL
{
    public class BakeryDbContext : DbContext
    {
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Permission> Permissions { get; set; } = null!;
        public DbSet<RolePermission> RolePermissions { get; set; } = null!;
        public DbSet<Branch> Branches { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Shift> Shifts { get; set; } = null!;
        public DbSet<EmployeeShift> EmployeeShifts { get; set; } = null!;

        public BakeryDbContext()
        {
        }

        public BakeryDbContext(DbContextOptions<BakeryDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = GetConnectionString();
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        public static string GetConnectionString()
        {
            string? environmentConnection = Environment.GetEnvironmentVariable("BAKERY_DB_CONNECTION");
            if (!string.IsNullOrWhiteSpace(environmentConnection))
            {
                return environmentConnection;
            }

            try
            {
                string basePath = AppContext.BaseDirectory;
                string jsonPath = Path.Combine(basePath, "appsettings.json");

                // If not found in bin directory, search in project root directory
                if (!File.Exists(jsonPath))
                {
                    string fallbackPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
                    if (File.Exists(fallbackPath))
                    {
                        basePath = Directory.GetCurrentDirectory();
                    }
                }

                var configuration = new ConfigurationBuilder()
                    .SetBasePath(basePath)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                    .Build();

                string? connStr = configuration.GetConnectionString("BakeryDbConnection");
                if (!string.IsNullOrWhiteSpace(connStr))
                {
                    return connStr;
                }
            }
            catch
            {
                // Fallback to default LocalDB connection if file reading fails
            }

            return "Server=(localdb)\\MSSQLLocalDB;Database=BanhMiShopDB;Trusted_Connection=True;TrustServerCertificate=True;";
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Role configuration
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(r => r.RoleID);
                entity.Property(r => r.RoleName).HasMaxLength(50).IsRequired();
                entity.HasIndex(r => r.RoleName).IsUnique();
                entity.Property(r => r.Description).HasMaxLength(255);
            });

            // 2. Permission configuration
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("Permissions");
                entity.HasKey(p => p.PermissionID);
                entity.Property(p => p.PermissionCode).HasMaxLength(50).IsRequired();
                entity.HasIndex(p => p.PermissionCode).IsUnique();
                entity.Property(p => p.PermissionName).HasMaxLength(100).IsRequired();
                entity.Property(p => p.GroupName).HasMaxLength(50).IsRequired();
                entity.Property(p => p.Description).HasMaxLength(255);
            });

            // 3. RolePermission configuration (Many-to-Many junction)
            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.ToTable("RolePermissions");
                entity.HasKey(rp => new { rp.RoleID, rp.PermissionID });

                entity.HasOne(rp => rp.Role)
                    .WithMany(r => r.RolePermissions)
                    .HasForeignKey(rp => rp.RoleID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rp => rp.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(rp => rp.PermissionID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // 4. Branch configuration
            modelBuilder.Entity<Branch>(entity =>
            {
                entity.ToTable("Branches");
                entity.HasKey(b => b.BranchID);
                entity.Property(b => b.BranchCode).HasMaxLength(20).IsRequired();
                entity.HasIndex(b => b.BranchCode).IsUnique();
                entity.Property(b => b.BranchName).HasMaxLength(150).IsRequired();
                entity.Property(b => b.Address).HasMaxLength(255);
                entity.Property(b => b.Phone).HasMaxLength(20);
                entity.Property(b => b.IsActive).HasDefaultValue(true);
                entity.Property(b => b.CreatedAt).HasDefaultValueSql("SYSDATETIME()");
            });

            // 5. Employee configuration
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employees");
                entity.HasKey(e => e.EmployeeID);
                entity.Property(e => e.EmployeeCode).HasMaxLength(20).IsRequired();
                entity.HasIndex(e => e.EmployeeCode).IsUnique();
                entity.Property(e => e.FullName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.DateOfBirth).HasColumnType("date");
                entity.Property(e => e.Address).HasMaxLength(255);
                entity.Property(e => e.Position).HasMaxLength(50).IsRequired();
                entity.Property(e => e.HireDate).HasColumnType("date").HasDefaultValueSql("CAST(GETDATE() AS DATE)");
                entity.Property(e => e.Status).HasMaxLength(20).IsRequired().HasDefaultValue("ACTIVE");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSDATETIME()");

                entity.HasOne(e => e.Branch)
                    .WithMany(b => b.Employees)
                    .HasForeignKey(e => e.BranchID)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);
            });

            // 6. User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(u => u.UserID);
                entity.Property(u => u.Username).HasMaxLength(50).IsRequired();
                entity.HasIndex(u => u.Username).IsUnique();
                entity.Property(u => u.PasswordHash).HasMaxLength(255).IsRequired();
                entity.Property(u => u.IsActive).HasDefaultValue(true);
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("SYSDATETIME()");
                entity.Property(u => u.FailedLoginCount).HasDefaultValue(0);
                entity.Property(u => u.LockoutEnd).HasColumnType("datetime2");

                entity.HasOne(u => u.Employee)
                    .WithOne(e => e.User)
                    .HasForeignKey<User>(u => u.EmployeeID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(u => u.EmployeeID).IsUnique();

                entity.HasOne(u => u.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(u => u.RoleID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // 7. Shift configuration
            modelBuilder.Entity<Shift>(entity =>
            {
                entity.ToTable("Shifts");
                entity.HasKey(s => s.ShiftID);
                entity.Property(s => s.ShiftCode).HasMaxLength(20).IsRequired();
                entity.HasIndex(s => s.ShiftCode).IsUnique();
                entity.Property(s => s.ShiftName).HasMaxLength(100).IsRequired();
                entity.Property(s => s.StartTime).HasColumnType("time").IsRequired();
                entity.Property(s => s.EndTime).HasColumnType("time").IsRequired();
                entity.Property(s => s.IsActive).HasDefaultValue(true);
                entity.Property(s => s.CreatedAt).HasDefaultValueSql("SYSDATETIME()");
            });

            // 8. EmployeeShift configuration
            modelBuilder.Entity<EmployeeShift>(entity =>
            {
                entity.ToTable("EmployeeShifts");
                entity.HasKey(es => es.EmployeeShiftID);
                entity.Property(es => es.WorkDate).HasColumnType("date").IsRequired();
                entity.Property(es => es.Status).HasMaxLength(20).IsRequired().HasDefaultValue("SCHEDULED");
                entity.Property(es => es.Note).HasMaxLength(255);

                entity.HasIndex(es => new { es.EmployeeID, es.ShiftID, es.WorkDate }).IsUnique();

                entity.HasOne(es => es.Employee)
                    .WithMany(e => e.EmployeeShifts)
                    .HasForeignKey(es => es.EmployeeID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(es => es.Shift)
                    .WithMany(s => s.EmployeeShifts)
                    .HasForeignKey(es => es.ShiftID)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
