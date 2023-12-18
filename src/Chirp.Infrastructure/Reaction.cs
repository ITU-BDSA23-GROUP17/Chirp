using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public class Reaction
{
    public required string CheepId { get; set; }
    public required string AuthorId { get; set; }
    public DateTime TimeStamp { get; set; }

}