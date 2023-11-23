
namespace Chirp.Core
{
    public interface IReactionRepository : IDisposable
    {

        public IEnumerable<ReactionDTO> GetReactionsFromCheepId(string cheepId);

        public IEnumerable<ReactionDTO> GetReactionsFromAuthorId(string authorId);

        void InsertReaction(CheepDTO CheepDTO, AuthorDTO AuthorDTO, ReactionTypeDTO ReactionTypeDTO);
        void DeleteReaction(string cheepId, string authorId);

        void UpdateReaction(string cheepId, string authorId);

        void Save();

    }
}