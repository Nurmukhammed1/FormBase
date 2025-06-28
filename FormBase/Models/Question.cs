using System.ComponentModel.DataAnnotations;

namespace FormBase.Models;

public class Question
{
    public int Id { get; set; }
    public string? Text { get; set; }
    public QuestionType Type { get; set; }
    public int Order { get; set; } 
    public bool IsRequired { get; set; } = false;
    public int TemplateId { get; set; }
    public Template Template { get; set; }
    public List<Answer> Answers { get; set; } = new List<Answer>();
} 