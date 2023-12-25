using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PropertyRentalManagement.Models;
using System.Reflection.Emit;

namespace PropertyRentalManagement.Data;

public partial class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    public virtual DbSet<Apartment> Apartments { get; set; }

    public virtual DbSet<AppUser> AppUsers { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Building> Buildings { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Size> Sizes { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("dbo");
        builder.Entity<AppUser>(entity =>
        {
            entity.ToTable(name: "AppUsers");
        });
        builder.Entity<IdentityRole>(entity =>
        {
            entity.ToTable(name: "Roles");
        });
        builder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.ToTable("UserRoles");
        });
        builder.Entity<IdentityUserClaim<string>>(entity =>
        {
            entity.ToTable("UserClaims");
        });
        builder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.ToTable("UserLogins");
        });
        builder.Entity<IdentityRoleClaim<string>>(entity =>
        {
            entity.ToTable("RoleClaims");
        });
        builder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.ToTable("UserTokens");
        });


        //
        builder.Entity<Appointment>(entity =>
        {
            entity.Property(e => e.PotentialTenantId).HasMaxLength(100);
            entity.Property(e => e.PropertyManagerId).HasMaxLength(100);
            entity.Property(e => e.ScheduledDate).HasColumnType("datetime");

            entity.HasOne(d => d.PotentialTenant).WithMany(p => p.AppointmentPotentialTenants)
                .HasForeignKey(d => d.PotentialTenantId)
                .HasConstraintName("FK_Appointments_AppUsers_TenantId");

            entity.HasOne(d => d.PropertyManager).WithMany(p => p.AppointmentPropertyManagers)
                .HasForeignKey(d => d.PropertyManagerId)
                .HasConstraintName("FK_Appointments_AppUsers_ManagerId");

            entity.HasOne(d => d.Status).WithMany(p => p.Appointments).HasForeignKey(d => d.StatusId);
        });

        builder.Entity<Building>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.PropertyOwnerId).HasMaxLength(100);

            entity.HasOne(d => d.PropertyOwner).WithMany(p => p.Buildings)
                .HasForeignKey(d => d.PropertyOwnerId)
                .HasConstraintName("FK_Buildings_AppUsers_OwnerId");
        });

        builder.Entity<Message>(entity =>
        {
            entity.Property(e => e.ReceiverId).HasMaxLength(100);
            entity.Property(e => e.SenderId).HasMaxLength(100);
            entity.Property(e => e.Timestamp).HasColumnType("datetime");

            entity.HasOne(d => d.ReadStatus).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ReadStatusId)
                .HasConstraintName("FK_Messages_Statuses_StatusId");

            entity.HasOne(d => d.Receiver).WithMany(p => p.MessageReceivers).HasForeignKey(d => d.ReceiverId);

            entity.HasOne(d => d.Sender).WithMany(p => p.MessageSenders).HasForeignKey(d => d.SenderId);
        });
        builder.Entity<Size>(entity =>
        {
            entity.Property(e => e.SizeId).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(50);
        });

        builder.Entity<Status>(entity =>
        {
            entity.Property(e => e.StatusId).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(50);
        });
        OnModelCreatingPartial(builder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
