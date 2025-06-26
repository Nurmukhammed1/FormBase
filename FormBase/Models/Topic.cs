using System.ComponentModel.DataAnnotations;

namespace FormBase.Models;

public class Topic
{
    public int Id { get; set; }
        
    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    public List<Template> Templates = new List<Template>();
}