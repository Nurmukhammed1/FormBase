namespace FormBase.Models;

public class Form
{
    public int Id { get; set; }
    public int TemplateId { get; set; }
    public Template Template { get; set; }
    public string AuthorId { get; set; }
    public User Author { get; set; }
    public DateTime FilledAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}