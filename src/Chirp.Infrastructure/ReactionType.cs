using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public class ReactionType
{
    public required string Id { get; set; }
   
    public required string Name { get; set; }

    public required string Icon { get; set; }

}