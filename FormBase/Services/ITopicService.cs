using FormBase.Models;

namespace FormBase.Services;

public interface ITopicService
{
    Task<IEnumerable<Topic>> GetTopicsAsync();
    Task<Topic> CreateTopicAsync(Topic topic);
    Task<Topic> DeleteTopicAsync(int topicId);
}