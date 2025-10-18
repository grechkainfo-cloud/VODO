using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Vodo.Application.Requests.Divisions.CreateDivision;
using Vodo.Application.Requests.Divisions.DeleteDivision;
using Vodo.Application.Requests.Divisions.UpdateDivision;
using Vodo.DAL.Context;
using Vodo.Models;
using Xunit;

namespace Vodo.UnitTests.Application.Requests.Divisions
{
    public class DivisionHandlersTests
    {
        private VodoContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<VodoContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new VodoContext(options);
        }

        [Fact]
        public async Task CreateDivision_Handler_Creates_Division_With_Name_And_Geometry()
        {
            // Arrange
            var context = CreateContext();
            var handler = new CreateDivisionCommandHandler(context);
            var command = new CreateDivisionCommand
            {
                Name = "Test Division",
                GeometryGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.5,20.5]}"
            };

            // Act
            var id = await handler.Handle(command, CancellationToken.None);

            // Assert
            var division = await context.Divisions.FindAsync(id);
            Assert.NotNull(division);
            Assert.Equal("Test Division", division!.Name);
            Assert.NotNull(division.Geometry);
            var point = division.Geometry as Point;
            Assert.NotNull(point);
            Assert.Equal(10.5, Math.Round(point!.X, 6));
            Assert.Equal(20.5, Math.Round(point.Y, 6));
            Assert.Equal(4326, point.SRID);
        }

        [Fact]
        public async Task UpdateDivision_Handler_Updates_Name_And_Geometry()
        {
            // Arrange
            var context = CreateContext();
            var initial = new Division { Name = "Before", Geometry = new Point(new Coordinate(1, 2)) { SRID = 4326 } };
            await context.Divisions.AddAsync(initial);
            await context.SaveChangesAsync();

            var handler = new UpdateDivisionCommandHandler(context);
            var cmd = new UpdateDivisionCommand
            {
                Id = initial.Id,
                Name = "After",
                GeometryGeoJson = "{\"type\":\"Point\",\"coordinates\":[5,6]}"
            };

            // Act
            var updatedId = await handler.Handle(cmd, CancellationToken.None);

            // Assert
            Assert.Equal(initial.Id, updatedId);
            var division = await context.Divisions.FindAsync(initial.Id);
            Assert.NotNull(division);
            Assert.Equal("After", division!.Name);
            var point = division.Geometry as Point;
            Assert.NotNull(point);
            Assert.Equal(5, Math.Round(point!.X, 6));
            Assert.Equal(6, Math.Round(point.Y, 6));
            Assert.Equal(4326, point.SRID);
        }

        [Fact]
        public async Task UpdateDivision_Handler_Throws_When_NotFound()
        {
            // Arrange
            var context = CreateContext();
            var handler = new UpdateDivisionCommandHandler(context);
            var cmd = new UpdateDivisionCommand { Id = Guid.NewGuid(), Name = "X" };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(cmd, CancellationToken.None));
        }

        [Fact]
        public async Task DeleteDivision_Handler_Removes_Division()
        {
            // Arrange
            var context = CreateContext();
            var division = new Division { Name = "ToDelete" };
            await context.Divisions.AddAsync(division);
            await context.SaveChangesAsync();

            var handler = new DeleteDivisionCommandHandler(context);

            // Act
            await handler.Handle(new DeleteDivisionCommand { Id = division.Id }, CancellationToken.None);

            // Assert
            var found = await context.Divisions.FindAsync(division.Id);
            Assert.Null(found);
        }

        [Fact]
        public async Task DeleteDivision_Handler_Throws_When_NotFound()
        {
            // Arrange
            var context = CreateContext();
            var handler = new DeleteDivisionCommandHandler(context);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(new DeleteDivisionCommand { Id = Guid.NewGuid() }, CancellationToken.None));
        }
    }
}