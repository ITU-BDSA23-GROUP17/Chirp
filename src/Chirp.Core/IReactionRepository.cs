
namespace Chirp.Core
{
    public interface IReactionRepository
    {

        public IEnumerable<ReactionDTO> GetReactionsFromCheepId(string cheepId);

        public IEnumerable<ReactionDTO> GetReactionsFromAuthorId(string authorId);

        void InsertReaction(ReactionDTO reaction); 
        void DeleteReaction(string cheepId, string authorId);

        void UpdateReaction(string cheepId, string authorId);

    }
}