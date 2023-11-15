using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Chirp.Core;

namespace Chirp.Infrastructure;

public class ReactionTypeRepository : IReactionTypeRepository, IDisposable
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public string GetReactionTypeIconById(string reactionTypeId)
    {
        throw new NotImplementedException();
    }

    public string GetReactionTypeNameById(string reactionTypeId)
    {
        throw new NotImplementedException();
    }
}