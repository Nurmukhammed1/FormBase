using FormBase.Models;

namespace FormBase.ViewModels;

public class MainPageViewModel
{
    public IEnumerable<Template> LatestTemplates { get; set; } = new List<Template>();
    public IEnumerable<Template> MostPopularTemplates { get; set; } = new List<Template>();
    public IEnumerable<Topic> Topics { get; set; } = new List<Topic>();
    public string? SearchTerm { get; set; }
    public IEnumerable<Template> SearchResults { get; set; } = new List<Template>();
    public bool IsSearchPerformed { get; set; }
}