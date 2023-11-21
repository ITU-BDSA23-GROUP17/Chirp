using System.Security.AccessControl;

namespace Chirp.Core
{
    public interface IFollowRepository : IDisposable
    {
        List<string> GetFollowerIDsByAuthorID(string AuthorID);
        List<string> GetFollowingIDsByAuthorID(string AuthorID);
        void InsertNewFollow(string FollowerID, string FollowingID);
        void RemoveFollow(string FollowerID, string FollowingID);
    }
}