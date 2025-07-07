using System.ComponentModel.DataAnnotations;
using NpgsqlTypes;

namespace FormBase.Models;

public class Template
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string AuthorId { get; set; }
    public User? Author { get; set; }
    public int TopicId { get; set; }
    public Topic? Topic { get; set; }
    public bool IsPublic { get; set; } = true;
    public List<Question> Questions { get; set; } = new List<Question>();
    public List<Form> Forms { get; set; } = new List<Form>();
    public List<TemplateUser> AllowedUsers { get; set; } = new List<TemplateUser>();
    public NpgsqlTsVector SearchVector { get; set; }
}