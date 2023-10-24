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
    public void test()
    {
        using var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);
        using var context = new ChirpDBContext(builder.Options);
        var repository = new CheepRepository(context);
    }
}
