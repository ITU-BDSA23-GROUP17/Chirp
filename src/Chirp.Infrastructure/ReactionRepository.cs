
using System.ComponentModel.DataAnnotations;
using System.Data;
using Chirp.Core;

namespace Chirp.Infrastructure
{
    public class ReactionRepository : IReactionRepository
    {

        private ChirpDBContext context;

        public Task InsertNewReactionAsync(string CheepId, string AuthorId, string ReactionTypeId)
        {
            context.Reactions.Add(new Reaction() { CheepId = CheepId, AuthorId = AuthorId, ReactionTypeId = ReactionTypeId, TimeStamp = DateTime.Now });
            return context.SaveChangesAsync();
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