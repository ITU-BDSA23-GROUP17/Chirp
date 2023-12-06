using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Chirp.Core;

namespace Chirp.Infrastructure
{
    public class HashtagRepository : IHashtagRepository
    {
        private ChirpDBContext context;

        public HashtagRepository(ChirpDBContext context)
        {
            this.context = context;
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

        public List<string> GetCheepIDsByHashtagText(string hashtagText)
        {
            var cheepIds = context.Hashtags
                .Where(h => h.HashtagText == hashtagText)
                .Select(h => h.CheepID)
                .ToList();

            return cheepIds;
        }

        public async Task InsertNewHashtagCheepPairingAsync(string HashtagText, string CheepID)
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

        public async Task<List<Hashtag>> GetHashtagsAsync()
        {
            var hashtags = await context.Hashtags.ToListAsync();
            return hashtags;
        }

        public async Task<List<string>> GetSortedPopularHashtagsAsync()
        {

            var hashtags = await GetHashtagsAsync();
            var popularHashtags = hashtags
                .GroupBy(h => new { h.HashtagText }) // Sort by hashtag text (by creating new anonymous type to sort by)
                .OrderByDescending(group => group.Count())
                .Select(group => group.Key.HashtagText) // Select hashtag text from the actual hashtag.
                .ToList();

            return popularHashtags;
        }
    }
}