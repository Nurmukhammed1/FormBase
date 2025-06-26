using System.ComponentModel.DataAnnotations;

namespace FormBase.Models;

public class Template
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Title is required")]
    [StringLength(60, ErrorMessage = "Title cannot exceed 60 characters")]
    public string? Title { get; set; }
    
    [Required(ErrorMessage = "Description is required")]
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }
    
    public string? ImageUrl { get; set; }
    
    [Display(Name = "Created Date")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Display(Name = "Updated Date")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    [Required]
    public string AuthorId { get; set; }
    public User Author { get; set; }
    
    [Required]
    public int TopicId { get; set; }
    public Topic Topic { get; set; }
    
    public bool IsPublic { get; set; } = true;

    public List<Question> Questions { get; set; } = new List<Question>();
    public List<Form> Forms { get; set; } = new List<Form>();
    public List<TemplateUser> AllowedUsers { get; set; } = new List<TemplateUser>();
}