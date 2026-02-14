using Pathly.DataModels;

namespace Pathly.Services.Interfaces
{
    public interface ITagService
    {
        public Task<IEnumerable<Tag>> GetUserTagsAsync(string userId); 
        public Task CreateTagAsync(string name, string userId); 
        public Task<bool> DeleteTagAsync(int id, string userId);
    }
}
