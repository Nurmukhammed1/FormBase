using System.ComponentModel.DataAnnotations;
using FormBase.Models;

namespace FormBase.ViewModels;

public class TemplateDetailViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool IsPublic { get; set; } = true;
    public Topic Topic { get; set; }
    public User Author { get; set; }
    public List<Question> Questions { get; set; } = new List<Question>();
    public bool CanEdit { get; set; }
}