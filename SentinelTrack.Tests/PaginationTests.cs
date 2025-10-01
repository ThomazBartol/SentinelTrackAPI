using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

public class PaginationTests
{
    [Fact]
    public void Yards_List_Returns_Paged_Data()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var db = new AppDbContext(options);
        db.Yards.Add(new Yard { Id = Guid.NewGuid(), Name = "A", Capacity = 10 });
        db.Yards.Add(new Yard { Id = Guid.NewGuid(), Name = "B", Capacity = 20 });
        db.SaveChanges();

        var pageSize = 1;
        var q = db.Yards.OrderBy(x => x.Name);
        var total = q.Count();
        var items = q.Skip(0).Take(pageSize).ToList();

        Assert.Equal(2, total);
        Assert.Single(items);
    }
}
