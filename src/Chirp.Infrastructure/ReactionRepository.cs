
using System.ComponentModel.DataAnnotations;
using System.Data;
using Chirp.Core;

namespace Chirp.Infrastructure
{
    public class ReactionRepository : IReactionRepository
    {

        private ChirpDBContext context;
        public ReactionRepository(ChirpDBContext dbContext)
        {
            context = dbContext;
        }
        public async Task InsertNewReactionAsync(string CheepId, string AuthorId, string ReactionTypeId)
        {
            context.Reactions.AddAsync(new Reaction() { CheepId = CheepId, AuthorId = AuthorId, ReactionTypeId = ReactionTypeId, TimeStamp = DateTime.Now });
            await context.SaveChangesAsync();
        }

        public Task RemoveReactionAsync(string CheepId, string AuthorId)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}