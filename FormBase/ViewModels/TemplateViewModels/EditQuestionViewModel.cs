using System.ComponentModel.DataAnnotations;
using FormBase.Models;

namespace FormBase.ViewModels;

public class EditQuestionViewModel
{
    public int? Id { get; set; } 
    
    [Required]
    [StringLength(100, ErrorMessage = "Question cannot exceed 100 characters")]
    public string Text { get; set; } = string.Empty;

    [Required]
    public QuestionType Type { get; set; }
    
    [Range(0, int.MaxValue)]
    public int Order { get; set; }

    public bool IsRequired { get; set; } = false;
    
    public bool IsDeleted { get; set; } = false;
}