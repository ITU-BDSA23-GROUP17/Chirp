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
            throw new NotImplementedException();
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
