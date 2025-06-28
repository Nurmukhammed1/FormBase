using FormBase.Data;
using FormBase.Models;
using Microsoft.EntityFrameworkCore;

namespace FormBase.Services;

public class TemplateService : ITemplateService
{
    private readonly ApplicationDbContext _context;

    public TemplateService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Template>> GetUserTemplatesAsync(string userId)
    {
        return await _context.Templates
            .Where(t => t.AuthorId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Template>> GetPublicTemplatesAsync()
    {
        return await _context.Templates
            .Where(t => t.IsPublic)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<Template?> GetTemplateByIdAsync(int id)
    {
        var template = await _context.Templates
            .FirstOrDefaultAsync(t => t.Id == id);

        return template;
    }

    public async Task<Template> CreateTemplateAsync(Template template)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            _context.Add(template);
            await _context.SaveChangesAsync();

            if (template.Questions.Any())
            {
                foreach (var question in template.Questions)
                {
                    question.TemplateId = template.Id;
                }

                await _context.SaveChangesAsync();
            }

            await transaction.CommitAsync();
            return template;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<Template> UpdateTemplateAsync(Template template)
    {
        _context.Templates.Update(template);
        await _context.SaveChangesAsync();
        return template;
    }

    public async Task<bool> DeleteTemplateAsync(int id)
    {
        var template = await _context.Templates.FindAsync(id);

        if (template == null)
        {
            return false;
        }
        
        _context.Templates.Remove(template);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CanUserAccesTemplateAsync(int templateId, string userId)
    {
        var template = await _context.Templates.FirstOrDefaultAsync(t => t.Id == templateId);

        if (template == null) return false;
        if (template.IsPublic || template.AuthorId == userId) return true;

        return template.AllowedUsers.Any(au => au.UserId == userId);
    }
}