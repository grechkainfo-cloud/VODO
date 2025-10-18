using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Vodo.Application.Requests.JobObjects.CreateJobObject;
using Vodo.Application.Requests.JobObjects.DeleteJobObject;
using Vodo.Application.Requests.JobObjects.UpdateJobObject;
using Vodo.DAL.Context;
using Vodo.Models;
using Xunit;

namespace Vodo.UnitTests.Application.Requests.JobObjects
{
    public class JobObjectHandlersTests
    {
        private VodoContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<VodoContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new VodoContext(options);
        }

        [Fact]
        public async Task CreateJobObject_Handler_Creates_JobObject_With_Name_Location_Address_OwnerDivision_DivisionId()
        {
            // Arrange
            var context = CreateContext();
            var handler = new CreateJobObjectCommandHandler(context);
            var divisionId = Guid.NewGuid();
            var command = new CreateJobObjectCommand
            {
                Name = "Test JobObject",
                PointGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.5,20.5]}",
                Address = new Address { Line1 = "Line1", City = "City" },
                OwnerDivision = "Ops",
                DivisionId = divisionId
            };

            // Act
            var id = await handler.Handle(command, CancellationToken.None);

            // Assert
            var jobObject = await context.JobObjects.FindAsync(id);
            Assert.NotNull(jobObject);
            Assert.Equal("Test JobObject", jobObject!.Name);
            Assert.NotNull(jobObject.Location);
            var point = jobObject.Location as Point;
            Assert.NotNull(point);
            Assert.Equal(10.5, Math.Round(point!.X, 6));
            Assert.Equal(20.5, Math.Round(point.Y, 6));
            Assert.Equal(4326, point.SRID);

            Assert.NotNull(jobObject.Address);
            Assert.Equal("Line1", jobObject.Address!.Line1);
            Assert.Equal("City", jobObject.Address.City);

            Assert.Equal("Ops", jobObject.OwnerDivision);
            Assert.Equal(divisionId, jobObject.DivisionId);
        }

        [Fact]
        public async Task UpdateJobObject_Handler_Updates_Name_Location_And_OtherFields()
        {
            // Arrange
            var context = CreateContext();
            var initial = new JobObject
            {
                Name = "Before",
                Location = new Point(new Coordinate(1, 2)) { SRID = 4326 },
                Address = new Address { Line1 = "Old" },
                OwnerDivision = "OldDiv"
            };
            await context.JobObjects.AddAsync(initial);
            await context.SaveChangesAsync();

            var handler = new UpdateJobObjectCommandHandler(context);
            var newDivisionId = Guid.NewGuid();
            var cmd = new UpdateJobObjectCommand
            {
                Id = initial.Id,
                Name = "After",
                // set Location to non-null so handler will process PointGeoJson
                Location = new Point(0, 0),
                PointGeoJson = "{\"type\":\"Point\",\"coordinates\":[5,6]}",
                Address = new Address { Line1 = "New" },
                OwnerDivision = "NewDiv",
                DivisionId = newDivisionId
            };

            // Act
            var updatedId = await handler.Handle(cmd, CancellationToken.None);

            // Assert
            Assert.Equal(initial.Id, updatedId);
            var jobObject = await context.JobObjects.FindAsync(initial.Id);
            Assert.NotNull(jobObject);
            Assert.Equal("After", jobObject!.Name);
            var point = jobObject.Location as Point;
            Assert.NotNull(point);
            Assert.Equal(5, Math.Round(point!.X, 6));
            Assert.Equal(6, Math.Round(point.Y, 6));
            Assert.Equal(4326, point.SRID);

            Assert.NotNull(jobObject.Address);
            Assert.Equal("New", jobObject.Address!.Line1);
            Assert.Equal("NewDiv", jobObject.OwnerDivision);
            Assert.Equal(newDivisionId, jobObject.DivisionId);
        }

        [Fact]
        public async Task UpdateJobObject_Handler_Throws_When_NotFound()
        {
            // Arrange
            var context = CreateContext();
            var handler = new UpdateJobObjectCommandHandler(context);
            var cmd = new UpdateJobObjectCommand { Id = Guid.NewGuid(), Name = "X" };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(cmd, CancellationToken.None));
        }

        [Fact]
        public async Task DeleteJobObject_Handler_Removes_JobObject()
        {
            // Arrange
            var context = CreateContext();
            var jobObject = new JobObject { Name = "ToDelete" };
            await context.JobObjects.AddAsync(jobObject);
            await context.SaveChangesAsync();

            var handler = new DeleteJobObjectCommandHandler(context);

            // Act
            await handler.Handle(new DeleteJobObjectCommand { Id = jobObject.Id }, CancellationToken.None);

            // Assert
            var found = await context.JobObjects.FindAsync(jobObject.Id);
            Assert.Null(found);
        }

        [Fact]
        public async Task DeleteJobObject_Handler_Throws_When_NotFound()
        {
            // Arrange
            var context = CreateContext();
            var handler = new DeleteJobObjectCommandHandler(context);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(new DeleteJobObjectCommand { Id = Guid.NewGuid() }, CancellationToken.None));
        }
    }
}