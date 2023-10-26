using System;
using System.Data.Common;
using System.Linq;
using System.Net.Sockets;
using Chirp.Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.VisualBasic;
using Xunit;
namespace Chirp.Infrastructure.Test;




//help from https://learn.microsoft.com/en-us/ef/core/testing/testing-without-the-database





public class InfrastructureUnitTests
{

    [Fact]
    public void InsertAuthorAddsAuthorToDatabase()
    {
        //Arrange
        using var connection = new SqliteConnection("Filename=:memory:");
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);
        connection.Open();

        using var context = new ChirpDBContext(builder.Options);
        context.initializeDB(); //ensure all tables are created
        IAuthorRepository authorRepository = new AuthorRepository(context);

        //Act
        authorRepository.InsertAuthor("Author Authorson", "authorson@author.com");
        context.SaveChanges(); //save changes to in memory database 


        // Assert
        var insertedAuthor = context.Authors.FirstOrDefault(a => a.Email == "authorson@author.com");
        Assert.NotNull(insertedAuthor); //check that we get an author
        Assert.Equal("Author Authorson", insertedAuthor.Name); //check that we have the right author

    }
    /*
    [Fact]
    public void InsertCheepAddsCheepToDatabase()
    {
        {
            // Arrange
            using var connection = new SqliteConnection("Filename=:memory:");
            var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);
            connection.Open();

            using var context = new ChirpDBContext(builder.Options);
            context.initializeDB(); //ensure all tables are created
            ICheepRepository cheepRepository = new CheepRepository(context);

            //We create a cheep
            var cheepDto = new CheepDTO(
                Id: "asdasd",
                Message: "test",
                TimeStamp: DateTime.Now,
                Author: "Helge");


            // Act
            cheepRepository.InsertCheep(cheepDto);
            context.SaveChanges(); // Save changes to in-memory database

            // Assert
            var insertedCheep = context.Cheeps.FirstOrDefault(c => c.Text == "This is a test cheep");
            Assert.NotNull(insertedCheep); // Check that we get a cheep
            Assert.Equal("This is a test cheep", insertedCheep.Text); // Check that we have the right cheep
        }
    }*/
}
