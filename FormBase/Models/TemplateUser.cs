namespace FormBase.Models;

public class TemplateUser
{
    public int TemplateId { get; set; }
    public Template Template { get; set; }
    
    public string UserId { get; set; }
    public User User { get; set; }
}