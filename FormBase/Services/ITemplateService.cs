using FormBase.Models;

namespace FormBase.Services;

public interface ITemplateService
{
    Task<IEnumerable<Template>> GetUserTemplatesAsync(string userId);
    Task<IEnumerable<Template>> GetPublicTemplatesAsync();
    Task<Template?> GetTemplateByIdAsync(int id);
    Task<Template> CreateTemplateAsync(Template template);
    Task<Template> UpdateTemplateAsync(Template template);
    Task<bool> DeleteTemplateAsync(int id);
    Task<bool> CanUserAccesTemplateAsync(int templateId, string userId);
}