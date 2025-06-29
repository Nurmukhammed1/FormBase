using FormBase.Models;
using FormBase.Services;
using FormBase.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FormBase.Controllers;

public class TemplatesController : Controller
{
    private ITemplateService _templateService;
    private UserManager<User> _userManager;
    private ITopicService _topicService;

    public TemplatesController(ITemplateService templateService, UserManager<User> userManager, ITopicService topicService)
    {
        _templateService = templateService;
        _userManager = userManager;
        _topicService = topicService;
    }
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> UserTemplates()
    {
        var userId = _userManager.GetUserId(User);
        var templates = await _templateService.GetUserTemplatesAsync(userId);
        return View(templates);
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var templates = await _templateService.GetPublicTemplatesAsync();
        return View(templates);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var template = await _templateService.GetTemplateByIdAsync(id);

        if (template == null) throw new TemplateNotFoundException(id);
        
        return View(template);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new CreateTemplateViewModel();
        await PopulateCreateViewModelAsync(model);
        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTemplateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateCreateViewModelAsync(model);
            return View(model);
        }
        
        var userId = _userManager.GetUserId(User);

        var template = new Template
        {
            Title = model.Title,
            Description = model.Description,
            ImageUrl = model.ImageUrl,
            IsPublic = model.IsPublic,
            AuthorId = userId,
            TopicId = model.TopicId
        };

        foreach (var questionViewModel in model.Questions)
        {
            var question = new Question
            {
                Text = questionViewModel.Text,
                Type = questionViewModel.Type,
                IsRequired = questionViewModel.IsRequired,
                Order = questionViewModel.Order
            };
                
            template.Questions.Add(question);
        }

        var result = await _templateService.CreateTemplateAsync(template);
            
        return RedirectToAction(nameof(Index));
    }
    
    private async Task PopulateCreateViewModelAsync(CreateTemplateViewModel model)
    {
        model.Topics = await _topicService.GetTopicsAsync();
        model.QuestionTypes = Enum.GetValues(typeof(QuestionType)).Cast<QuestionType>().ToList();
    }
}