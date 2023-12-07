
namespace Chirp.Core
{
    public interface IReactionRepository
    {

        Task InsertNewReactionAsync(string CheepId, string AuthorId, string ReactionTypeId);

        Task RemoveReactionAsync(string CheepId, string AuthorId);

        Task<List<AuthorDTO>> GetAuthorListReactionByCheepId(string CheepId);

        Task<List<string>> GetCheepIdsByAuthorId(string AuthorId);

        Task<Boolean> CheckIfAuthorReactedToCheep(string CheepId, string AuthorId);

        Task<string> GetTotalReactionsByCheepId(string CheepId);


        void Save();

    }
}