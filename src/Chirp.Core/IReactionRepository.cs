
namespace Chirp.Core
{
    public interface IReactionRepository
    {

        Task InsertNewReactionAsync(string CheepId, string AuthorId, string ReactionTypeId);

        Task RemoveReactionAsync(string CheepId, string AuthorId);

        Task<List<string>> GetAuthorListReactionByCheepId(string CheepId);



        void Save();

    }
}