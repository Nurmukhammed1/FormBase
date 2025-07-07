using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FormBase.Models;

namespace FormBase.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
    public DbSet<Template> Templates { get; set; }
    public DbSet<Topic> Topics { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Form> Forms { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<TemplateUser> TemplateUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<TemplateUser>()
            .HasKey(tu => new { tu.TemplateId, tu.UserId });

        builder.Entity<TemplateUser>()
            .HasOne(tu => tu.Template)
            .WithMany(t => t.AllowedUsers)
            .HasForeignKey(tu => tu.TemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<TemplateUser>()
            .HasOne(tu => tu.User)
            .WithMany()
            .HasForeignKey(tu => tu.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        
        builder.Entity<Template>()
            .HasOne(t => t.Author)
            .WithMany(u => u.Templates)
            .HasForeignKey(t => t.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Template>()
            .HasOne(t => t.Topic)
            .WithMany(tp => tp.Templates)
            .HasForeignKey(t => t.TopicId)
            .OnDelete(DeleteBehavior.Restrict);
        
        
        builder.Entity<Question>()
            .HasOne(q => q.Template)
            .WithMany(t => t.Questions)
            .HasForeignKey(q => q.TemplateId)
            .OnDelete(DeleteBehavior.Cascade);
        
        
        builder.Entity<Form>()
            .HasOne(f => f.Template)
            .WithMany(t => t.Forms)
            .HasForeignKey(f => f.TemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Form>()
            .HasOne(f => f.Author)
            .WithMany(u => u.Forms)
            .HasForeignKey(f => f.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Answer>()
            .HasOne(a => a.Question)
            .WithMany(q => q.Answers)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Answer>()
            .HasOne(a => a.Form)
            .WithMany(f => f.Answers)
            .HasForeignKey(a => a.FormId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<Template>()
            .HasIndex(t => t.AuthorId);

        builder.Entity<Template>()
            .HasIndex(t => t.TopicId);

        builder.Entity<Form>()
            .HasIndex(f => f.TemplateId);

        builder.Entity<Form>()
            .HasIndex(f => f.AuthorId);

        builder.Entity<Question>()
            .HasIndex(q => q.TemplateId);

        builder.Entity<Answer>()
            .HasIndex(a => a.QuestionId);
        
        builder.Entity<Answer>()
            .HasIndex(a => a.FormId);


        builder.Entity<Template>()
            .HasGeneratedTsVectorColumn(
                t => t.SearchVector,
                "english",
                t => new { t.Title, t.Description })
            .HasIndex(b => b.SearchVector)
            .HasMethod("GIN");
    }
}