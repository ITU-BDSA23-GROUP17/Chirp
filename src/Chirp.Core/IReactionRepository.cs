
namespace Chirp.Core
{
    public interface IReactionRepository
    {

        Task InsertNewReactionAsync(string CheepId, string AuthorId);

        Task RemoveReactionAsync(string CheepId, string AuthorId);

        Task<List<string>> GetCheepIdsByAuthorId(string AuthorId);

        Task<Boolean> CheckIfAuthorReactedToCheep(string CheepId, string AuthorId);


        void Save();

    }
}