using FormBase.Data;
using FormBase.Models;
using Microsoft.EntityFrameworkCore;

namespace FormBase.Services;

public class TopicService : ITopicService
{
    private readonly ApplicationDbContext _context;

    public TopicService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Topic>> GetTopicsAsync()
    {
        return await _context.Topics.ToListAsync();
    }

    public async Task<Topic> CreateTopicAsync(Topic topic)
    {
        _context.Topics.Add(topic);
        await _context.SaveChangesAsync();
        return topic;
    }

    public Task<Topic> DeleteTopicAsync(int topicId)
    {
        throw new NotImplementedException();
    }
}