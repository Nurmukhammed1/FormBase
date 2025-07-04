using FormBase.Models;
using FormBase.Services;
using FormBase.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FormBase.Controllers;

public class FormsController : Controller
{
    private readonly IFormService _formService;
    private readonly UserManager<User> _userManager;
    private readonly ITemplateService _templateService;

    public FormsController(IFormService formService, UserManager<User> userManager, ITemplateService templateService)
    {
        _formService = formService;
        _userManager = userManager;
        _templateService = templateService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var forms = await _formService.GetUserFormsAsync(userId);
        return View(forms);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Create(int templateId)
    {
        var template = await _templateService.GetTemplateByIdAsync(templateId);
        
        var model = new CreateEditFormViewModel
        {
            TemplateId = templateId,
            Questions = template.Questions
        };
        
        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateEditFormViewModel model)
    {
        var userId = _userManager.GetUserId(User);

        var form = new Form
        {
            TemplateId = model.TemplateId,
            AuthorId = userId,
            Answers = model.Answers
        };

        await _formService.CreateFormAsync(form);

        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var form = await _formService.GetFormByIdAsync(id);
        var userId = _userManager.GetUserId(User);

        var model = new FormDetailViewModel
        {
            Id = form.Id,
            Template = form.Template,
            Author = form.Author,
            Answers = form.Answers,
            CanEdit = CanUserEditForm(form, userId)
        };

        return View(model);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var form = await _formService.GetFormByIdAsync(id);
        var userId = _userManager.GetUserId(User);
        
        if (!CanUserEditForm(form, userId)) return Forbid();
        
        var model = new CreateEditFormViewModel
        {
            Id = form.Id,
            TemplateId = form.TemplateId,
            Questions = form.Template.Questions,
            Answers = form.Answers
        };
        
        return View(model);
    }
    
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CreateEditFormViewModel model)
    {
        var form = await _formService.GetFormByIdAsync(id);
        if (form == null) return NotFound();
        
        var userId = _userManager.GetUserId(User);
        
        if (!CanUserEditForm(form, userId)) return Forbid();
        
        form.Answers = model.Answers;

        await _formService.UpdateFormAsync(form);
            
        return RedirectToAction(nameof(Details), new { id = form.Id });
    }
    
    
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var form = await _formService.GetFormByIdAsync(id);
        if (form == null) return NotFound();
        
        var userId = _userManager.GetUserId(User);
        
        if (!CanUserEditForm(form, userId)) return Forbid();

        var result = await _formService.DeleteFormAsync(id);
        if (!result)
        {
            TempData["ErrorMessage"] = "Failed to delete the form. Please try again.";
            return RedirectToAction(nameof(Details), new { id });
        }
            
        TempData["SuccessMessage"] = "Form deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
    
    
    private bool CanUserEditForm(Form form, string userId)
    {
        if (User.IsInRole("Admin") || form.AuthorId == userId) 
            return true;

        return false;
    }
}