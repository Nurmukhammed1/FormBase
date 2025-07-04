using FormBase.Models;

namespace FormBase.Services;

public interface IFormService
{
    Task<IEnumerable<Form>> GetUserFormsAsync(string userId);
    Task<Form?> GetFormByIdAsync(int id);
    Task<IEnumerable<Form>> GetFormsOfTemplateAsync(int templateId);
    Task<Form> CreateFormAsync(Form form);
    Task<Form> UpdateFormAsync(Form form);
    Task<bool> DeleteFormAsync(int id);
    
}