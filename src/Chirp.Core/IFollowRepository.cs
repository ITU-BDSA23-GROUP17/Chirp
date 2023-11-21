using System.Security.AccessControl;

namespace Chirp.Core
{
    public interface IFollowRepository : IDisposable
    {
        List<AuthorDTO> GetFollowersByAuthorID(string AuthorID);
        List<AuthorDTO> GetFollowsByAuthorID(string AuthorID);
        void InsertNewFollow(string FollowerID, string FollowingID);
        void RemoveFollow(string FollowerID, string FollowingID);
    }
}