
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
            return context.Reactions.Where(r => r.CheepId == cheepId).Select(r => new ReactionDTO(r.CheepId, r.AuthorId, r.TimeStamp, r.ReactionTypeId)).ToList();

        }

        public void InsertReaction(CheepDTO CheepDTO, AuthorDTO AuthorDTO, ReactionTypeDTO ReactionTypeDTO)
        {
            var author = context.Authors.Find(CheepDTO.AuthorId) ?? throw new Exception("Author could not be found by AuthorID");
            var cheep = context.Cheeps.Find(CheepDTO.Id) ?? throw new Exception("Cheep could not be found by CheepID");
            var reactionType = context.ReactionTypes.Find(ReactionTypeDTO.Id) ?? throw new Exception("ReactionType could not be found by ReactionTypeID");
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