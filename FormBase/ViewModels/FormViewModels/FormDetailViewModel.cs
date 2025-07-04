using FormBase.Models;

namespace FormBase.ViewModels;

public class FormDetailViewModel
{
    public int Id { get; set; }
    public Template Template { get; set; }
    public User Author { get; set; }
    public DateTime FilledAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public List<Answer> Answers { get; set; } = new List<Answer>();
    public bool CanEdit { get; set; }
}