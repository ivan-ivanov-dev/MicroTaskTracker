using Microsoft.EntityFrameworkCore;
using MicroTaskTracker.Data;
using MicroTaskTracker.Models.DBModels;
using MicroTaskTracker.Services.Interfaces;

namespace MicroTaskTracker.Services.Implementations
{
    public class TagService : ITagService
    {
        private readonly ApplicationDbContext _context; 
        public TagService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task CreateTagAsync(string name, string userId)
        {
            bool tagExists = await _context.Tags.AnyAsync(t => t.UserId == userId && t.Name.ToLower() == name.ToLower()); 
            if(tagExists)
            {
                throw new InvalidOperationException("A tag with the same name already exists!");
            }
            var tag = new Tag
            {
                Name = name.Trim(),
                UserId = userId
            };

            _context.Tags.Add(tag); 
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteTagAsync(int id, string userId)
        {
            var tag = await _context.Tags.FindAsync(id); 
            if(tag == null)
            {
                return false;
            }
            if(tag.UserId != userId)
            {
                throw new UnauthorizedAccessException(); 
            }
            
            _context.Tags.Remove(tag); 
            await _context.SaveChangesAsync();
            
            return true;
        }

        public async Task<IEnumerable<Tag>> GetUserTagsAsync(string userId)
        {
            var tags = await _context.Tags
                .Where(t => t.UserId == userId)
                .OrderBy(t => t.Name)
                .ToListAsync();
            return tags;
        }
    }
}
