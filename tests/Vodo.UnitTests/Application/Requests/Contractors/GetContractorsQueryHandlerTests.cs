using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Vodo.Application.Requests.Contractors.GetContractors;
using Vodo.DAL.Context;
using Vodo.Models;
using Xunit;

namespace Vodo.UnitTests.Application.Requests.Contractors
{
    public class GetContractorsQueryHandlerTests
    {
        private VodoContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<VodoContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new VodoContext(options);
        }

        [Fact]
        public async Task Handle_Returns_All_Contractors_From_Db()
        {
            // Arrange
            await using var context = CreateContext();
            var c1 = new Contractor { Name = "C1", Inn = "111" };
            var c2 = new Contractor { Name = "C2", Inn = "222" };
            await context.Contractors.AddRangeAsync(c1, c2);
            await context.SaveChangesAsync();

            var handler = new GetContractorsQueryHandler(context);

            // Act
            var result = await handler.Handle(new GetContractorsQuery(), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            var list = result.ToList();
            Assert.Contains(list, x => x.Name == "C1" && x.Inn == "111");
            Assert.Contains(list, x => x.Name == "C2" && x.Inn == "222");
        }
    }
}