namespace Chirp.Core
{
    public interface IFollowRepository
    {
        Task<List<string>> GetFollowerIDsByAuthorIDAsync(string AuthorID);
        Task<List<string>> GetFollowingIDsByAuthorIDAsync(string AuthorID);
        Task<int> GetFollowerCountByAuthorIDAsync(string AuthorID);
        Task<int> GetFollowingCountByAuthorIDAsync(string AuthorID);
        Task InsertNewFollowAsync(string FollowerID, string FollowingID);
        Task RemoveFollowAsync(string FollowerID, string FollowingID);
    }
}