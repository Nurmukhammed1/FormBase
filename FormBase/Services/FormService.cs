using FormBase.Data;

namespace FormBase.Services;

public class FormService : IFormSevice
{
    private readonly ApplicationDbContext _context;

    public FormService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    
}