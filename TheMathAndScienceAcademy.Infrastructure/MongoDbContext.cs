
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
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<TeachingAssignment> TeachingAssignments => Set<TeachingAssignment>();

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

            entity.HasOne<Role>()
                .WithMany()
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Academy>()
                .WithMany()
                .HasForeignKey(x => x.AcademyId)
                .OnDelete(DeleteBehavior.Restrict);
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

            entity.HasOne<Academy>()
                .WithMany()
                .HasForeignKey(x => x.AcademyId)
                .OnDelete(DeleteBehavior.Restrict);
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
            entity.Property(x => x.ProfilePhotoKey).HasMaxLength(100);
            entity.Property(x => x.ProfilePhotoContentType).HasMaxLength(100);
            entity.HasIndex(x => new { x.AcademyId, x.EnrollmentNumber }).IsUnique();
            entity.HasIndex(x => x.AcademyId);
            entity.HasIndex(x => x.UserId).IsUnique().HasFilter("\"UserId\" IS NOT NULL");

            entity.HasOne<Academy>()
                .WithMany()
                .HasForeignKey(x => x.AcademyId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.SetNull);
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

            entity.HasOne<Academy>()
                .WithMany()
                .HasForeignKey(x => x.AcademyId)
                .OnDelete(DeleteBehavior.Restrict);
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

            entity.HasOne<Student>()
                .WithMany()
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<AcademicClass>()
                .WithMany()
                .HasForeignKey(x => x.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<Academy>()
                .WithMany()
                .HasForeignKey(x => x.AcademyId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Action).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Entity).HasMaxLength(100).IsRequired();
            entity.Property(x => x.ActorUserId).HasMaxLength(50);
            entity.Property(x => x.EntityId).HasMaxLength(50);
            entity.Property(x => x.AcademyId).HasMaxLength(50);
            entity.Property(x => x.Result).HasMaxLength(30).IsRequired();
            entity.HasIndex(x => new { x.AcademyId, x.CreatedAt });
            entity.HasIndex(x => x.ActorUserId);
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(x => x.Id); entity.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.EmployeeNumber).HasMaxLength(50).IsRequired(); entity.Property(x => x.Email).HasMaxLength(255);
            entity.Property(x => x.PhoneNumber).HasMaxLength(30); entity.Property(x => x.Gender).HasMaxLength(30);
            entity.Property(x => x.Address).HasMaxLength(500); entity.Property(x => x.Status).HasMaxLength(30).IsRequired();
            entity.Property(x => x.AcademyId).HasMaxLength(50).IsRequired(); entity.Property(x => x.UserId).HasMaxLength(50);
            entity.Property(x => x.ProfilePhotoKey).HasMaxLength(100); entity.Property(x => x.ProfilePhotoContentType).HasMaxLength(100);
            entity.HasIndex(x => new { x.AcademyId, x.EmployeeNumber }).IsUnique(); entity.HasIndex(x => x.UserId).IsUnique().HasFilter("\"UserId\" IS NOT NULL");
            entity.HasOne<Academy>().WithMany().HasForeignKey(x => x.AcademyId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(x => x.Id); entity.Property(x => x.Name).HasMaxLength(150).IsRequired(); entity.Property(x => x.Code).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500); entity.Property(x => x.AcademicYear).HasMaxLength(30).IsRequired();
            entity.Property(x => x.Status).HasMaxLength(30).IsRequired(); entity.Property(x => x.AcademyId).HasMaxLength(50).IsRequired();
            entity.HasIndex(x => new { x.AcademyId, x.Code, x.AcademicYear }).IsUnique(); entity.HasIndex(x => new { x.AcademyId, x.Name, x.AcademicYear }).IsUnique();
            entity.HasOne<Academy>().WithMany().HasForeignKey(x => x.AcademyId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TeachingAssignment>(entity =>
        {
            entity.HasKey(x => x.Id); entity.Property(x => x.TeacherId).HasMaxLength(50).IsRequired(); entity.Property(x => x.SubjectId).HasMaxLength(50).IsRequired();
            entity.Property(x => x.ClassId).HasMaxLength(50).IsRequired(); entity.Property(x => x.AcademyId).HasMaxLength(50).IsRequired();
            entity.Property(x => x.AcademicYear).HasMaxLength(30).IsRequired(); entity.Property(x => x.Status).HasMaxLength(30).IsRequired(); entity.Property(x => x.AssignmentRole).HasMaxLength(30).IsRequired();
            entity.HasIndex(x => new { x.AcademyId, x.ClassId, x.SubjectId, x.AcademicYear });
            entity.HasIndex(x => new { x.AcademyId, x.TeacherId, x.Status });
            entity.HasOne<Teacher>().WithMany().HasForeignKey(x => x.TeacherId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Subject>().WithMany().HasForeignKey(x => x.SubjectId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<AcademicClass>().WithMany().HasForeignKey(x => x.ClassId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Academy>().WithMany().HasForeignKey(x => x.AcademyId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
