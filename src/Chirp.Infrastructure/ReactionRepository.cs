
using System.ComponentModel.DataAnnotations;
using System.Data;
using Chirp.Core;

namespace Chirp.Infrastructure
{
    public class ReactionRepository : IReactionRepository, IDisposable
    {

        private ChirpDBContext context;

        public ReactionRepository(ChirpDBContext context)
        {
            this.context = context;
        }

        public void DeleteReaction(string cheepId, string authorId)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ReactionDTO> GetReactionsFromAuthorId(string authorId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ReactionDTO> GetReactionsFromCheepId(string cheepId)
        {
            throw new NotImplementedException();
        }

        public void InsertReaction(ReactionDTO reaction)
        {
            throw new NotImplementedException();
        }

        public void UpdateReaction(string cheepId, string authorId)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}