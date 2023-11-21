using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Chirp.Core;

namespace Chirp.Infrastructure;

public class FollowRepository : IFollowRepository, IDisposable
{
    private ChirpDBContext context;
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public List<AuthorDTO> GetFollowersByAuthorID(string AuthorID)
    {
        var followerIDs = context.Followings
        .Where(f => f.FollowingId == AuthorID)
        .Select(f => f.FollowerId)
        .ToList();

        using (var authorRepository = new AuthorRepository(context))
        {
            var followers = authorRepository.GetAuthorsByIds(followerIDs);
            return followers;
        }
    }


    public List<AuthorDTO> GetFollowingsByAuthorID(string AuthorID)
    {
        throw new NotImplementedException();
    }

    public void InsertNewFollow(string FollowerID, string FollowingID)
    {
        context.Followings.Add(new Follow() { FollowerId = FollowerID, FollowingId = FollowingID, Timestamp = DateTime.Now });
    }

    public void RemoveFollow(string FollowerID, string FollowingID)
    {
        var follow = context.Followings.Find(FollowerID, FollowingID);
        if (follow != null)
        {
            context.Remove(follow);
        }
    }
}