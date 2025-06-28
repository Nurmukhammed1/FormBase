using FormBase.Models;
using FormBase.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FormBase.Controllers;

public class FormsController : Controller
{
    private IFormSevice _formSevice;
    private UserManager<User> _userManager;

    public FormsController(IFormSevice formSevice, UserManager<User> userManager)
    {
        _formSevice = formSevice;
        _userManager = userManager;
    }
}