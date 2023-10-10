using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Chirp.Objects;

{
    public class CheepRepository : ICheepRepository, IDisposable
{
    private CheepContext context;

    public CheepRepository(CheepContext context)
    {
        this.context = context;
    }

    public IEnumerable<Cheep> GetCheeps()
    {
        return context.Cheeps.ToList();
    }

    public Cheep GetCheepByID(int id)
    {
        return context.Cheeps.Find(id);
    }

    public void InsertCheep(Cheep cheep)
    {
        context.Cheeps.Add(cheep);
    }

    public void DeleteCheep(int cheepID)
    {
        Cheep cheep = context.cheeps.Find(cheepID);
        context.cheeps.Remove(cheep);
    }

    void UpdateCheep(Cheep cheep);
    {
        context.Entry(cheep).State = EntityState.Modified;
    }

public void Save()
{
    context.SaveChanges();
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
}
}