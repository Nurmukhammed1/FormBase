using FormBase.Data;
using FormBase.Models;
using Microsoft.EntityFrameworkCore;

namespace FormBase.Services;

public class TemplateService : ITemplateService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TemplateService> _logger;

    public TemplateService(ApplicationDbContext context, ILogger<TemplateService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Template>> GetUserTemplatesAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentException("User Id cannot be null or empty!", nameof(userId));
        
        return await _context.Templates
            .Where(t => t.AuthorId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Template>> GetPublicTemplatesAsync()
    {
        return await _context.Templates
            .Where(t => t.IsPublic)
            .Include(t => t.Author)
            .Include(t => t.Topic)
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
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync();
            throw new InvalidOperationException("Failed to create template due to databse error.", ex);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new InvalidOperationException("An unexpected error occured while creating the template.", ex);
        }
    }

    public async Task<Template> UpdateTemplateAsync(Template template)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            _context.Update(template);
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
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync();
            throw new DbUpdateException("Failed to create template due to databse error.", ex);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new InvalidOperationException("An unexpected error occured while creating the template.", ex);
        }
    }

    public async Task<bool> DeleteTemplateAsync(int id)
    {
        var template = await _context.Templates.FindAsync(id);

        if (template == null) return false;
        
        _context.Templates.Remove(template);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CanUserAccesTemplateAsync(int templateId, string userId)
    {
        if (string.IsNullOrEmpty(userId)) return false;

        var template = await _context.Templates
            .Include(t => t.AllowedUsers)
            .FirstOrDefaultAsync(t => t.Id == templateId);

        if (template == null) return false;
        if (template.IsPublic || template.AuthorId == userId) return true;

        return template.AllowedUsers.Any(au => au.UserId == userId);
    }
}