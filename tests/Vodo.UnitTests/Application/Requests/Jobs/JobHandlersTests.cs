using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Vodo.Application.Requests.Jobs.CreateJob;
using Vodo.Application.Requests.Jobs.DeleteJob;
using Vodo.Application.Requests.Jobs.GetJobs;
using Vodo.Application.Requests.Jobs.UpdateJob;
using Vodo.DAL.Context;
using Vodo.Models;
using Xunit;

namespace Vodo.UnitTests.Application.Requests.Jobs
{
    public class JobHandlersTests
    {
        private VodoContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<VodoContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new VodoContext(options);
        }

        [Fact]
        public async Task CreateJob_Handler_Creates_Job_With_All_Fields()
        {
            // Arrange
            var context = CreateContext();

            // Ensure referenced JobObject exists
            var jobObject = new JobObject { Name = "Site A" };
            await context.JobObjects.AddAsync(jobObject);
            await context.SaveChangesAsync();

            var handler = new CreateJobCommandHandler(context);
            var command = new CreateJobCommand
            {
                Title = "Excavate trench",
                Description = "Detailed description",
                Type = JobType.Excavation,
                StatusId = 1,
                Priority = JobPriority.High,
                ContractorId = null,
                JobObjectId = jobObject.Id,
                GeometryGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.5,20.5]}"
            };

            // Act
            var id = await handler.Handle(command, CancellationToken.None);

            // Assert
            var job = await context.Jobs.FindAsync(id);
            Assert.NotNull(job);
            Assert.Equal("Excavate trench", job!.Title);
            Assert.Equal("Detailed description", job.Description);
            Assert.Equal(JobType.Excavation, job.Type);
            Assert.Equal(1, job.StatusId);
            Assert.Equal(JobPriority.High, job.Priority);
            Assert.Equal(jobObject.Id, job.JobObjectId);
            Assert.NotNull(job.Geometry);
            var point = job.Geometry as Point;
            Assert.NotNull(point);
            Assert.Equal(10.5, Math.Round(point!.X, 6));
            Assert.Equal(20.5, Math.Round(point.Y, 6));
            Assert.Equal(4326, point.SRID);
        }

        [Fact]
        public async Task UpdateJob_Handler_Updates_Fields()
        {
            // Arrange
            var context = CreateContext();

            // Seed job
            var job = new Job
            {
                Title = "Before",
                Description = "Old",
                Type = JobType.Other,
                StatusId = 5,
                Priority = JobPriority.Normal,
                JobObjectId = Guid.NewGuid()
            };
            await context.Jobs.AddAsync(job);
            await context.SaveChangesAsync();

            var handler = new UpdateJobCommandHandler(context);
            var cmd = new UpdateJobCommand
            {
                Id = job.Id,
                Title = "After",
                Description = "New",
                Type = JobType.PipelineRepair,
                StatusId = 2,
                Priority = JobPriority.Low,
                ContractorId = Guid.NewGuid(),
                JobObjectId = job.JobObjectId,
                Geometry = new Point(new Coordinate(5, 6)) { SRID = 4326 },
                DaysOverdue = 3
            };

            // Act
            var updatedId = await handler.Handle(cmd, CancellationToken.None);

            // Assert
            Assert.Equal(job.Id, updatedId);
            var updated = await context.Jobs.FindAsync(job.Id);
            Assert.NotNull(updated);
            Assert.Equal("After", updated!.Title);
            Assert.Equal("New", updated.Description);
            Assert.Equal(JobType.PipelineRepair, updated.Type);
            Assert.Equal(2, updated.StatusId);
            Assert.Equal(JobPriority.Low, updated.Priority);
            Assert.NotNull(updated.Geometry);
            var p = updated.Geometry as Point;
            Assert.NotNull(p);
            Assert.Equal(5, Math.Round(p!.X, 6));
            Assert.Equal(6, Math.Round(p.Y, 6));
            Assert.Equal(4326, p.SRID);
            Assert.Equal(3, updated.DaysOverdue);
        }

        [Fact]
        public async Task UpdateJob_Handler_Throws_When_NotFound()
        {
            // Arrange
            var context = CreateContext();
            var handler = new UpdateJobCommandHandler(context);
            var cmd = new UpdateJobCommand { Id = Guid.NewGuid(), Title = "X" };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(cmd, CancellationToken.None));
        }

        [Fact]
        public async Task GetJobs_Handler_Returns_All_Jobs()
        {
            // Arrange
            await using var context = CreateContext();
            var j1 = new Job { Title = "J1", Type = JobType.Other, StatusId = 1, JobObjectId = Guid.NewGuid() };
            var j2 = new Job { Title = "J2", Type = JobType.Emergency, StatusId = 2, JobObjectId = Guid.NewGuid() };
            await context.Jobs.AddRangeAsync(j1, j2);
            await context.SaveChangesAsync();

            var handler = new GetJobsQueryHandler(context);

            // Act
            var result = await handler.Handle(new GetJobsQuery(), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            var list = result.ToList();
            Assert.Contains(list, x => x.Title == "J1");
            Assert.Contains(list, x => x.Title == "J2");
        }

        [Fact]
        public async Task DeleteJob_Handler_Removes_Job()
        {
            // Arrange
            var context = CreateContext();
            var job = new Job { Title = "ToDelete", Type = JobType.Other, StatusId = 1, JobObjectId = Guid.NewGuid() };
            await context.Jobs.AddAsync(job);
            await context.SaveChangesAsync();

            var handler = new DeleteJobCommandHandler(context);
            var command = new DeleteJobCommand { Id = job.Id };

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            var found = await context.Jobs.FindAsync(job.Id);
            Assert.Null(found);
        }

        [Fact]
        public async Task DeleteJob_Handler_Throws_When_NotFound()
        {
            // Arrange
            var context = CreateContext();
            var handler = new DeleteJobCommandHandler(context);
            var nonExistentId = Guid.NewGuid();
            var command = new DeleteJobCommand { Id = nonExistentId };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}
