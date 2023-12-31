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

        public async Task RemoveHashtagAsync(string hashtagText, string cheepID)
        {
            var hashtag = await context.Hashtags.FindAsync(hashtagText, cheepID);
            if (hashtag != null)
            {
                context.Remove(hashtag);
                await context.SaveChangesAsync();
            }
        }

        public List<string> GetPopularHashtags(List<string> hashtags)
        {
            var popularHashtags = hashtags
                .GroupBy(h => new { h }) // Sort by hashtag text (by creating new anonymous type to sort by)
                .OrderByDescending(group => group.Count())
                .Select(group => group.Key.h) // Select hashtag text from the actual hashtag.
                .Take(10) // We return at most 10 hashtags
                .ToList();

            return popularHashtags;
        }
    }
}
