using Microsoft.EntityFrameworkCore;

namespace Auth.Infra.Data;
public class AuthDbContext : DbContext
{
    public DbSet<Domain.Entities.Client> Clients { get; set; }
    public DbSet<Domain.Entities.Employee> Employees { get; set; }

    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);

        ConfigureClient(modelBuilder);
        ConfigureEmployee(modelBuilder);
    }

    private static void ConfigureClient(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Entities.Client>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Role).IsRequired().HasConversion<string>().HasMaxLength(50);

            entity.OwnsOne(e => e.Cpf, cpf =>
            {
                cpf.Property(c => c.Value)
                    .HasColumnName("Cpf")
                    .IsRequired()
                    .HasMaxLength(14);
            });

            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Cpf).IsUnique();

            entity.ToTable("clients");
        });
    }
    private static void ConfigureEmployee(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Entities.Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Role).IsRequired().HasConversion<string>().HasMaxLength(20);

            entity.HasIndex(e => e.Email).IsUnique();

            entity.ToTable("employees");
        });
    }
}
