using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Chirp.Core;

namespace Chirp.Infrastructure;

public class FollowRepository : IFollowRepository, IDisposable
{
    private ChirpDBContext context;

    public FollowRepository(ChirpDBContext context)
    {
        this.context = context;
    }
    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                context.Dispose();
            }
        }
        this.disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }


    public List<string> GetFollowerIDsByAuthorID(string AuthorID)
    {
        var followerIDs = context.Followings
        .Where(f => f.FollowingId == AuthorID)
        .Select(f => f.FollowerId)
        .ToList();
        return followerIDs;
    }

    public List<string> GetFollowingIDsByAuthorID(string AuthorID)
    {
        var followingIDs = context.Followings
        .Where(f => f.FollowerId == AuthorID)
        .Select(f => f.FollowingId)
        .ToList();

        return followingIDs;
    }

    public void InsertNewFollow(string FollowerID, string FollowingID)
    {
        context.Followings.Add(new Follow() { FollowerId = FollowerID, FollowingId = FollowingID, Timestamp = DateTime.Now });
        context.SaveChanges();
    }

    public void RemoveFollow(string FollowerID, string FollowingID)
    {
        var follow = context.Followings.Find(FollowerID, FollowingID);
        if (follow != null)
        {
            context.Remove(follow);
            context.SaveChanges();
        }
    }
}