using EchoVibe.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EchoVibe.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Like> Likes => Set<Like>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Follow> Follows => Set<Follow>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Like>()
            .HasIndex(l => new { l.ProjectId, l.UserId })
            .IsUnique();
            builder.Entity<Follow>()
    .HasIndex(f => new { f.FollowerId, f.FollowingId })
    .IsUnique();

builder.Entity<Follow>()
    .HasOne(f => f.Follower)
    .WithMany()
    .HasForeignKey(f => f.FollowerId)
    .OnDelete(DeleteBehavior.Restrict);

builder.Entity<Follow>()
    .HasOne(f => f.Following)
    .WithMany()
    .HasForeignKey(f => f.FollowingId)
    .OnDelete(DeleteBehavior.Restrict);
    }
}
