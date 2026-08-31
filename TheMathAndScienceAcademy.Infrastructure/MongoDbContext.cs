
using Microsoft.EntityFrameworkCore;
using TheMathAndScienceAcademy.Domain.Entities;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<Academy> Academies => Set<Academy>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<AcademicClass> AcademicClasses => Set<AcademicClass>();
    public DbSet<StudentEnrollment> StudentEnrollments => Set<StudentEnrollment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(255).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(255).IsRequired();
            entity.Property(x => x.ResetPasswordToken).HasMaxLength(200);
            entity.Property(x => x.RefreshToken).HasMaxLength(200);
            entity.Property(x => x.RoleId).HasMaxLength(50).IsRequired();
            entity.Property(x => x.AcademyId).HasMaxLength(50);
            entity.Property(x => x.IsActive).IsRequired();
            entity.Property(x => x.MustChangePassword).IsRequired();
            entity.Property(x => x.CredentialsSentAt);
            entity.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.AcademyId).HasMaxLength(50);
            entity.HasIndex(x => x.Name)
                .IsUnique()
                .HasFilter("\"AcademyId\" IS NULL");
            entity.HasIndex(x => new { x.Name, x.AcademyId })
                .IsUnique()
                .HasFilter("\"AcademyId\" IS NOT NULL");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(x => new { x.RoleId, x.PermissionId });
            entity.Property(x => x.IsGranted).IsRequired().HasDefaultValue(false);

            entity.HasOne(x => x.Role)
                .WithMany(x => x.RolePermissions)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Permission)
                .WithMany(x => x.RolePermissions)
                .HasForeignKey(x => x.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Academy>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(255).IsRequired();
            entity.Property(x => x.PhoneNumber).HasMaxLength(30).IsRequired();
            entity.Property(x => x.Address).HasMaxLength(500).IsRequired();
            entity.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.EnrollmentNumber).HasMaxLength(50).IsRequired();
            entity.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(255);
            entity.Property(x => x.PhoneNumber).HasMaxLength(30);
            entity.Property(x => x.Gender).HasMaxLength(30);
            entity.Property(x => x.Address).HasMaxLength(500);
            entity.Property(x => x.Status).HasMaxLength(30).IsRequired();
            entity.Property(x => x.AcademyId).HasMaxLength(50).IsRequired();
            entity.Property(x => x.UserId).HasMaxLength(50);
            entity.HasIndex(x => new { x.AcademyId, x.EnrollmentNumber }).IsUnique();
            entity.HasIndex(x => x.AcademyId);
            entity.HasIndex(x => x.UserId).IsUnique().HasFilter("\"UserId\" IS NOT NULL");
        });

        modelBuilder.Entity<AcademicClass>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Grade).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Section).HasMaxLength(50).IsRequired();
            entity.Property(x => x.AcademicYear).HasMaxLength(30).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.Status).HasMaxLength(30).IsRequired();
            entity.Property(x => x.AcademyId).HasMaxLength(50).IsRequired();
            entity.HasIndex(x => new { x.AcademyId, x.Grade, x.Section, x.AcademicYear }).IsUnique();
        });

        modelBuilder.Entity<StudentEnrollment>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.StudentId).HasMaxLength(50).IsRequired();
            entity.Property(x => x.ClassId).HasMaxLength(50).IsRequired();
            entity.Property(x => x.AcademyId).HasMaxLength(50).IsRequired();
            entity.Property(x => x.AcademicYear).HasMaxLength(30).IsRequired();
            entity.Property(x => x.Status).HasMaxLength(30).IsRequired();
            entity.HasIndex(x => new { x.StudentId, x.AcademicYear, x.AcademyId, x.Status });
            entity.HasIndex(x => x.ClassId);
            entity.HasIndex(x => x.AcademyId);
        });
    }
}
