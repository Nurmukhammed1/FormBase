using System.ComponentModel.DataAnnotations;
using FormBase.Models;

namespace FormBase.ViewModels;

public class CreateTemplateViewModel
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(60, ErrorMessage = "Title cannot exceed 60 characters")]
    public string Title { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Description is required")]
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; } = string.Empty;
    
    [StringLength(100)]
    [Display(Name = "Image URL")]
    [Url(ErrorMessage = "Please enter a valid URL")]
    public string? ImageUrl { get; set; }
    
    [Display(Name = "Public Template")]
    public bool IsPublic { get; set; } = true;
    
    [Range(1, int.MaxValue, ErrorMessage = "Please select a topic")]
    public int TopicId { get; set; }

    public IEnumerable<Topic> Topics { get; set; } = new List<Topic>();
    public IEnumerable<QuestionType> QuestionTypes { get; set; } = new List<QuestionType>();
    public List<CreateQuestionViewModel> Questions { get; set; } = new List<CreateQuestionViewModel>();
}

public class CreateQuestionViewModel
{
    [Microsoft.Build.Framework.Required]
    [StringLength(100, ErrorMessage = "Question cannot exceed 100 characters")]
    public string Text { get; set; } = string.Empty;

    [Required]
    public QuestionType Type { get; set; }
    
    [Range(0, int.MaxValue)]
    public int Order { get; set; }

    public bool IsRequired { get; set; } = false;
}