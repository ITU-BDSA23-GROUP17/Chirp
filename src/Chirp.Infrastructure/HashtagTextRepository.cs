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
