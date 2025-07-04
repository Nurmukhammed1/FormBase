using FormBase.Models;

namespace FormBase.ViewModels;

public class CreateEditFormViewModel
{
    public int Id { get; set; }
    public int TemplateId { get; set; }
    public List<Question> Questions { get; set; } = new List<Question>();
    public List<Answer> Answers { get; set; } = new List<Answer>();
}