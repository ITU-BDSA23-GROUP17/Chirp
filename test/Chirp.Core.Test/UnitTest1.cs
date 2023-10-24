using System;
using System.Data.Common;
using System.Linq;
using Chirp.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.VisualBasic;
using Xunit;
namespace Chirp.Core.Test;





public class test1
{

    [Fact]
    public void test()
    {
        using var connection = new SqliteConnection("Filename=:memory:");
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);
        connection.Open();

        using var context = new ChirpDBContext(builder.Options);
        ICheepRepository repository = new CheepRepository(context);
        Console.WriteLine(repository.GetCheeps(1));
    }
}
