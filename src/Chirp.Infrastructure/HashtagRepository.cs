using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Chirp.Core;

namespace Chirp.Infrastructure
{
    public class HashtagRepository : IHashtagRepository, IDisposable
    {
        private ChirpDBContext context;

        public HashtagRepository(ChirpDBContext context)
        {
            this.context = context;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task<List<string>> GetCheepIDsByHashtag(string Hashtag)
        {
            throw new NotImplementedException();
        }

        public async Task InsertNewHashtagFromCheep(string HashtagText, string CheepID)
        {
            context.Hashtags.Add(new Hashtag() { HashtagText = HashtagText, CheepID = CheepID });
            await context.SaveChangesAsync();
        }

        public async Task RemoveHashtag(string Hashtag)
        {
            var hashtag = await context.Hashtags.FindAsync(Hashtag);
            if (hashtag != null)
            {
                context.Remove(hashtag);
                await context.SaveChangesAsync();
            }
        }
    }
}