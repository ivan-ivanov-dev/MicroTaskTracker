using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pathly.Models.DBModels;

namespace Pathly.Data
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
        }
        public DbSet<TaskItem> Tasks => Set<TaskItem>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<TaskTag> TaskTags => Set<TaskTag>();
        public DbSet<Goal> Goals => Set<Goal>();
        public DbSet<ActionItem> Actions => Set<ActionItem>();
        public DbSet<Roadmap> Roadmaps => Set<Roadmap>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // TaskTag (many-to-many)
            builder.Entity<TaskTag>()
                .HasKey(tt => new { tt.TaskId, tt.TagId });

            builder.Entity<TaskTag>()
                .HasOne(tt => tt.Task)
                .WithMany(t => t.TaskTags)
                .HasForeignKey(tt => tt.TaskId);

            builder.Entity<TaskTag>()
                .HasOne(tt => tt.Tag)
                .WithMany(t => t.TaskTags)
                .HasForeignKey(tt => tt.TagId);

            // Tag -> User
            builder.Entity<Tag>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Goal -> User
            builder.Entity<Goal>()
                .HasOne(g => g.User)
                .WithMany()
                .HasForeignKey(g => g.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Roadmap -> User
            builder.Entity<Roadmap>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Roadmap -> Goal (ONE-TO-ONE)
            builder.Entity<Roadmap>()
                .HasOne(r => r.Goal)
                .WithOne()
                .HasForeignKey<Roadmap>(r => r.GoalId)
                .OnDelete(DeleteBehavior.Cascade);

            // Roadmap -> Actions (ONE-TO-MANY)
            builder.Entity<Roadmap>()
                .HasMany(r => r.Actions)
                .WithOne(a => a.Roadmap)
                .HasForeignKey(a => a.RoadmapId)
                .OnDelete(DeleteBehavior.Cascade);

            // Action -> Tasks (ONE-TO-MANY, nullable on Task)
            builder.Entity<ActionItem>()
                .HasMany(a => a.Tasks)
                .WithOne(t => t.Action)
                .HasForeignKey(t => t.ActionId)
                .OnDelete(DeleteBehavior.SetNull);
        }

    }
}
