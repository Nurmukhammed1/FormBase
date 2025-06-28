using FormBase.Models;
using FormBase.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormBase.Controllers;

public class TopicsController : Controller
{
    private readonly ITopicService _topicService;

    public TopicsController(ITopicService topicService)
    {
        _topicService = topicService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var topics = await _topicService.GetTopicsAsync();
        return View(topics);
    }
    
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Topic topic)
    {
        if (ModelState.IsValid)
        {
            var result = _topicService.CreateTopicAsync(topic);
            return RedirectToAction(nameof(Index));
        }

        return View(topic);
    }
}