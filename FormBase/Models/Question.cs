using System.ComponentModel.DataAnnotations;

namespace FormBase.Models;

public class Question
{
    public int Id { get; set; }
    
    [Required] 
    [StringLength(100)]
    public string? Text { get; set; }

    [Required]
    public QuestionType Type { get; set; }
    
    [Range(0, int.MaxValue)]
    public int Order { get; set; } 
    
    public bool IsRequired { get; set; } = false;
    
    [Required]
    public int TemplateId { get; set; }
    public Template Template { get; set; }
    
    public List<Answer> Answers { get; set; } = new List<Answer>();
} 