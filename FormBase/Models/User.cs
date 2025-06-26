using Microsoft.AspNetCore.Identity;

namespace FormBase.Models;

public class User : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsAdmin { get; set; } = false;
    public bool IsBlocked { get; set; } = false;

    public List<Template> Templates { get; set; } = new List<Template>();
    public List<Form> Forms { get; set; } = new List<Form>();
}