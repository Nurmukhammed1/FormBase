using FormBase.Data;
using FormBase.Models;
using Microsoft.EntityFrameworkCore;

namespace FormBase.Services;

public class FormService : IFormService
{
    private readonly ApplicationDbContext _context;

    public FormService(ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<IEnumerable<Form>> GetUserFormsAsync(string userId)
    {
        return await _context.Forms
            .Where(f => f.AuthorId == userId)
            .Include(f => f.Answers)
            .Include(f => f.Author)
            .Include(f => f.Template)
            .OrderByDescending(f => f.FilledAt)
            .ToListAsync();
    }

    public async Task<Form?> GetFormByIdAsync(int id)
    {
        return await _context.Forms
            .Include(f => f.Answers)
            .Include(f => f.Author)
            .Include(f => f.Template)
            .ThenInclude(t => t.Questions)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<IEnumerable<Form>> GetFormsOfTemplateAsync(int templateId)
    {
        return await _context.Forms
            .Where(f => f.TemplateId == templateId)
            .Include(f => f.Answers)
            .Include(f => f.Author)
            .Include(f => f.Template)
            .OrderByDescending(f => f.FilledAt)
            .ToListAsync();

    }

    public async Task<Form> CreateFormAsync(Form form)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            _context.Add(form);
            await _context.SaveChangesAsync();

            if (form.Answers.Any())
            {
                foreach (var answer in form.Answers)
                {
                    answer.FormId = form.Id;
                }

                await _context.SaveChangesAsync();
            }

            await transaction.CommitAsync();
            return form;
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync();
            throw new InvalidOperationException("Failed to create form due to database error.", ex);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new InvalidOperationException("An unexpected error occured while creating the form.", ex);
        }
    }

    public async Task<Form> UpdateFormAsync(Form form)
    {
        _context.Forms.Update(form);
        await _context.SaveChangesAsync();

        return form;
    }

    public async Task<bool> DeleteFormAsync(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var form = await _context.Forms
                .Include(f => f.Answers)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (form == null) throw new InvalidOperationException($"Form with ID {id} not found.");
            
            _context.Answers.RemoveRange(form.Answers);
            _context.Forms.Remove(form);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }
}