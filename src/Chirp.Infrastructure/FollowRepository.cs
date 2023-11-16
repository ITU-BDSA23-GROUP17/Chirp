using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Chirp.Core;

namespace Chirp.Infrastructure;

public class FollowRepository : IFollowRepository, IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public List<AuthorDTO> GetFollowersByAuthorID(string AuthorID)
    {
        throw new NotImplementedException();
    }

    public void InsertNewFollow(string FollowerID, string FollowingID)
    {
        throw new NotImplementedException();
    }

    public void RemoveFollow(string FollowerID, string FollowingID)
    {
        throw new NotImplementedException();
    }
}