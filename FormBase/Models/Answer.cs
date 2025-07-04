namespace FormBase.Models;

public class Answer
{
    public int Id { get; set; }
    public string? StringValue { get; set; }
    public string? TextValue { get; set; }
    public int? IntegerValue { get; set; }
    public bool? BooleanValue { get; set; }
    
    public int QuestionId { get; set; }
    public Question Question { get; set; }
    
    public int FormId { get; set; }
    public Form Form { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}