using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Vodo.Application.Requests.Jobs.CreateJob;
using Vodo.Application.Requests.Jobs.DeleteJob;
using Vodo.Application.Requests.Jobs.GetJobs;
using Vodo.Application.Requests.Jobs.UpdateJob;
using Vodo.Models;
using Vodo.Server.Controllers;
using Xunit;

namespace Vodo.UnitTests.Controllers
{
    public class JobsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<JobsController>> _loggerMock;
        private readonly JobsController _controller;

        public JobsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<JobsController>>();
            _controller = new JobsController(_mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetList_ReturnsOk_WithJobs()
        {
            var items = new List<Job>
            {
                new Job { Title = "Job1", Type = JobType.Other, StatusId = 1, JobObjectId = Guid.NewGuid() },
                new Job { Title = "Job2", Type = JobType.Inspection, StatusId = 2, JobObjectId = Guid.NewGuid() }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetJobsQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync((IEnumerable<Job>)items);

            var result = await _controller.GetList();

            var action = Assert.IsType<ActionResult<IEnumerable<Job>>>(result);
            var ok = Assert.IsType<OkObjectResult>(action.Result);
            Assert.Equal(items, ok.Value);
        }

        [Fact]
        public async Task Create_ReturnsCreated_OnSuccess()
        {
            var newId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateJobCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(newId);

            var command = new CreateJobCommand
            {
                Title = "New Job",
                Type = JobType.Other,
                StatusId = 1,
                JobObjectId = Guid.NewGuid()
            };

            var result = await _controller.Create(command);

            var action = Assert.IsType<ActionResult<Guid>>(result);
            var created = Assert.IsType<CreatedAtActionResult>(action.Result);
            Assert.Equal(newId, created.Value);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_IfIdMismatch()
        {
            var cmd = new UpdateJobCommand { Id = Guid.NewGuid(), Title = "X" };
            var result = await _controller.Update(Guid.NewGuid(), cmd);

            var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("Id in route and body do not match", bad.Value.ToString());
        }

        [Fact]
        public async Task Update_ReturnsOk_OnSuccess()
        {
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateJobCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(id);

            var cmd = new UpdateJobCommand { Id = id, Title = "Updated" };
            var result = await _controller.Update(id, cmd);

            var action = Assert.IsType<ActionResult<Guid>>(result);
            var ok = Assert.IsType<OkObjectResult>(action.Result);
            Assert.Equal(id, ok.Value);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenHandlerThrowsKeyNotFound()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateJobCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new KeyNotFoundException());

            var id = Guid.NewGuid();
            var cmd = new UpdateJobCommand { Id = id, Title = "X" };
            var result = await _controller.Update(id, cmd);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_OnSuccess()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteJobCommand>(), It.IsAny<CancellationToken>()))
                         .Returns(Task.FromResult(Unit.Value));

            var id = Guid.NewGuid();
            var result = await _controller.Delete(id);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenHandlerThrowsKeyNotFound()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteJobCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.Delete(Guid.NewGuid());

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
