using FormBase.Models;

namespace FormBase.Services;

public interface ITemplateService
{
    Task<IEnumerable<Template>> GetUserTemplatesAsync(string userId);
    Task<IEnumerable<Template>> GetPublicTemplatesAsync();
    Task<IEnumerable<Template>> SearchTemplatesAsync(string searchTerm);
    Task<IEnumerable<Template>> GetLatestTemplatesAsync(int count = 12);
    Task<IEnumerable<Template>> GetMostPopularTemplatesAsync(int count = 5);
    Task<IEnumerable<Template>> GetTemplatesByTopicAsync(int topicId);
    Task<IEnumerable<Topic>> GetTopicsWithTemplateCountAsync();
    Task<Template?> GetTemplateByIdAsync(int id);
    Task<Template> CreateTemplateAsync(Template template);
    Task<Template> UpdateTemplateAsync(Template template);
    Task<bool> DeleteTemplateAsync(int id);
    Task<bool> CanUserAccesTemplateAsync(int templateId, string userId);
}