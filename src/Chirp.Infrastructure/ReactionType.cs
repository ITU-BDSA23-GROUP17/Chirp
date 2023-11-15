using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public class ReactionType
{
    public required string ReactionTypeId { get; set; }
   
    public required string ReactionTypeName { get; set; }

    public required string ReactionTypeIcon { get; set; }

}