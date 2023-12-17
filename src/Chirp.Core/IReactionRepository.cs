
namespace Chirp.Core
{
    public interface IReactionRepository
    {

        Task InsertNewReactionAsync(string CheepId, string AuthorId);

        Task RemoveReactionAsync(string CheepId, string AuthorId);

        Task<List<AuthorDTO>> GetAuthorListReactionByCheepId(string CheepId);

        Task<List<string>> GetCheepIdsByAuthorId(string AuthorId);

        Task<Boolean> CheckIfAuthorReactedToCheep(string CheepId, string AuthorId);

        public Task<List<string>> GetReactionByCheepId(string CheepId);


        void Save();

    }
}