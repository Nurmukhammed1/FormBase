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
    
    [HttpGet]
    public async Task<IActionResult> Index(string searchTerm)
    {
        var model = new MainPageViewModel
        {
            SearchTerm = searchTerm,
            IsSearchPerformed = !string.IsNullOrEmpty(searchTerm)
        };

        if (!string.IsNullOrEmpty(searchTerm))
        {
            model.SearchResults = await _templateService.SearchTemplatesAsync(searchTerm);
        }
        else
        {
            model.LatestTemplates = await _templateService.GetLatestTemplatesAsync();
            model.MostPopularTemplates = await _templateService.GetMostPopularTemplatesAsync();
        }

        model.Topics = await _templateService.GetTopicsWithTemplateCountAsync();

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> TopicTemplates(int id)
    {
        var topic = await _templateService.GetTopicsWithTemplateCountAsync();
        var selectedTopic = topic.FirstOrDefault(t => t.Id == id);

        if (selectedTopic == null)
        {
            return NotFound();
        }

        var templates = await _templateService.GetTemplatesByTopicAsync(id);

        var model = new TopicTemplatesViewModel
        {
            Topic = selectedTopic,
            Templates = templates
        };

        return View(model);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> UserTemplates()
    {
        var userId = _userManager.GetUserId(User);
        var templates = await _templateService.GetUserTemplatesAsync(userId);
        return View(templates);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var template = await _templateService.GetTemplateByIdAsync(id);

        if (template == null) throw new TemplateNotFoundException(id);

        var userId = _userManager.GetUserId(User);
        var canEdit = CanUserEditTemplate(template, userId);

        var model = new TemplateDetailViewModel()
        {
            Id = template.Id,
            Title = template.Title,
            Description = template.Description,
            ImageUrl = template.ImageUrl,
            IsPublic = template.IsPublic,
            Topic = template.Topic,
            Author = template.Author,
            Questions = template.Questions,
            CanEdit = canEdit
        };
        
        return View(model);
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
            
        return RedirectToAction(nameof(UserTemplates));
    }
    
    private async Task PopulateCreateViewModelAsync(CreateTemplateViewModel model)
    {
        model.Topics = await _topicService.GetTopicsAsync();
        model.QuestionTypes = Enum.GetValues(typeof(QuestionType)).Cast<QuestionType>().ToList();
    }


    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var template = await _templateService.GetTemplateByIdAsync(id);
        
        if (template == null) throw new TemplateNotFoundException(id);

        var userId = _userManager.GetUserId(User);
        
        if (!CanUserEditTemplate(template, userId)) return Forbid();
        
        var model = new EditTemplateViewModel
        {
            Id = template.Id,
            Title = template.Title ?? string.Empty,
            Description = template.Description ?? string.Empty,
            ImageUrl = template.ImageUrl,
            IsPublic = template.IsPublic,
            TopicId = template.TopicId,
            Questions = template.Questions.OrderBy(q => q.Order).Select(q => new EditQuestionViewModel
            {
                Id = q.Id,
                Text = q.Text ?? string.Empty,
                Type = q.Type,
                Order = q.Order,
                IsRequired = q.IsRequired
            }).ToList()
        };

        await PopulateEditViewModelAsync(model);
        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditTemplateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateEditViewModelAsync(model);
            return View(model);
        }

        var template = await _templateService.GetTemplateByIdAsync(id);
        
        var userId = _userManager.GetUserId(User);
        
        if (!CanUserEditTemplate(template, userId)) return Forbid();
        
        template.Title = model.Title;
        template.Description = model.Description;
        template.ImageUrl = model.ImageUrl;
        template.IsPublic = model.IsPublic;
        template.TopicId = model.TopicId;
        template.UpdatedAt = DateTime.UtcNow;
        
        MapQuestions(template, model.Questions);
        await _templateService.UpdateTemplateAsync(template);
        
        return RedirectToAction(nameof(Details), new { id = model.Id });
    }
    
    private void MapQuestions(Template template, List<EditQuestionViewModel> editedQuestions)
    {
        var existingQuestions = template.Questions.ToList();

        var editedQuestionsToDelete = editedQuestions
            .Where(q => q.IsDeleted)
            .ToList();

        var questionsToDelete = existingQuestions
            .Where(eq => editedQuestionsToDelete.Any(ed => ed.Id == eq.Id))
            .ToList();

        foreach (var question in questionsToDelete)
        {
            template.Questions.Remove(question);
        }
        
        foreach (var questionViewModel in editedQuestions.Where(q => !q.IsDeleted))
        {
            if (questionViewModel.Id.HasValue)
            {
                var existingQuestion = existingQuestions.FirstOrDefault(q => q.Id == questionViewModel.Id.Value);
                if (existingQuestion != null)
                {
                    existingQuestion.Text = questionViewModel.Text;
                    existingQuestion.Type = questionViewModel.Type;
                    existingQuestion.Order = questionViewModel.Order;
                    existingQuestion.IsRequired = questionViewModel.IsRequired;
                }
            }
            else
            {
                var newQuestion = new Question
                {
                    Text = questionViewModel.Text,
                    Type = questionViewModel.Type,
                    Order = questionViewModel.Order,
                    IsRequired = questionViewModel.IsRequired,
                    TemplateId = template.Id
                };
                template.Questions.Add(newQuestion);
            }
        }
    }

    private async Task PopulateEditViewModelAsync(EditTemplateViewModel model)
    {
        model.Topics = await _topicService.GetTopicsAsync();
        model.QuestionTypes = Enum.GetValues(typeof(QuestionType)).Cast<QuestionType>().ToList();
    }


    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var template = await _templateService.GetTemplateByIdAsync(id);
        if (template == null) return NotFound();
        
        var userId = _userManager.GetUserId(User);
        
        if (!CanUserEditTemplate(template, userId)) return Forbid();

        var result = await _templateService.DeleteTemplateAsync(id);

        if (!result)
        {
            return RedirectToAction(nameof(Details), new { id });
        }
        
        return RedirectToAction(nameof(UserTemplates));
    }
    

    [HttpGet]
    public IActionResult AddQuestion(bool isEdit = false)
    {
        var questionIndex = DateTimeOffset.UtcNow.Ticks;
    
        ViewData["Index"] = questionIndex;
        ViewData["IsEdit"] = isEdit;
        ViewBag.QuestionTypes = Enum.GetValues(typeof(QuestionType)).Cast<QuestionType>().ToList();

        if (isEdit)
        {
            return PartialView("_EditQuestionPartial", new EditQuestionViewModel());
        }
    
        return PartialView("_QuestionPartial", new CreateQuestionViewModel());
    }

    [Authorize]
    [HttpDelete]
    public IActionResult RemoveQuestion()
    {
        return Content("");
    }
    
    
    private bool CanUserEditTemplate(Template template, string userId)
    {
        if (User.IsInRole("Admin") || template.AuthorId == userId) 
            return true;

        return false;
    }
}