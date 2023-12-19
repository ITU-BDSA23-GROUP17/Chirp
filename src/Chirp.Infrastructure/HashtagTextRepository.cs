using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Chirp.Core;

namespace Chirp.Infrastructure
{
    public class HashtagTextRepository : IHashtagTextRepository
    {
        private ChirpDBContext context;

        public HashtagTextRepository(ChirpDBContext context)
        {
            this.context = context;
        }

        public async Task AddHashtag(string HashtagText)
        {
            var hashtag = await context.HashtagTexts.FindAsync(HashtagText);
            if (hashtag != null)
            {
                return;
            }
            context.HashtagTexts.Add(new HashtagText() { HashtagText_ = HashtagText });
            await context.SaveChangesAsync();
        }

        public async Task RemoveHashtag(string HashtagText)
        {
            var hashtag = await context.HashtagTexts.FindAsync(HashtagText);
            if (hashtag != null)
            {
                context.Remove(hashtag);
                await context.SaveChangesAsync();
            }
        }

        public Task<List<string>> GetUniqueHashtagTextsAsync()
        {
            var hashtags = context.Hashtags
                .Select(h => h.HashtagText)
                .Distinct()
                .ToListAsync();

            return hashtags;
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
