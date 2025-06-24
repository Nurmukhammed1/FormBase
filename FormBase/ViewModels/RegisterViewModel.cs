namespace FormBase.ViewModels;
using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required(ErrorMessage = "First name is required.")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }
    
    [Required(ErrorMessage = "Last name is required.")]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(40, MinimumLength = 8, ErrorMessage = "The {0} must be at {2} and at max {1} characters long.")]
    [DataType(DataType.Password)]
    [Compare("ConfirmPassword", ErrorMessage = "Password does not match.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Confirm Password is required.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; }

}