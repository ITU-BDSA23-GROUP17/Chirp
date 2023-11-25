
using System.ComponentModel.DataAnnotations;
using System.Data;
using Chirp.Core;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure
{
    public class ReactionRepository : IReactionRepository
    {

        private ChirpDBContext context;
        public ReactionRepository(ChirpDBContext dbContext)
        {
            context = dbContext;
        }

        public async Task<List<string>> GetReactionOnCheepByAuthorId(string CheepId)
        {
            var getAuthorIdOnCheep = await context.Reactions
            .Where(r => r.CheepId == CheepId)
            .Select(r => r.AuthorId)
            .ToListAsync();
            return getAuthorIdOnCheep;
        }

        public async Task InsertNewReactionAsync(string CheepId, string AuthorId, string ReactionTypeId)
        {
            context.Reactions.AddAsync(new Reaction() { CheepId = CheepId, AuthorId = AuthorId, ReactionTypeId = ReactionTypeId, TimeStamp = DateTime.Now });
            await context.SaveChangesAsync();
        }

        public async Task RemoveReactionAsync(string CheepId, string AuthorId)
        {
            var reaction = await context.Reactions.FindAsync(CheepId, AuthorId);
            if (reaction != null)
            {
                context.Remove(reaction);
                await context.SaveChangesAsync();
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}