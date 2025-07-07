using FormBase.Models;

namespace FormBase.ViewModels;

public class TopicTemplatesViewModel
{
    public Topic Topic { get; set; }
    public IEnumerable<Template> Templates { get; set; } = new List<Template>();
}