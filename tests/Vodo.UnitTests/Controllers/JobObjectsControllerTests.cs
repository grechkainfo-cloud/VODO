using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Vodo.Application.Requests.JobObjects.CreateJobObject;
using Vodo.Application.Requests.JobObjects.DeleteJobObject;
using Vodo.Application.Requests.JobObjects.GetJobObjects;
using Vodo.Application.Requests.JobObjects.UpdateJobObject;
using Vodo.Models;
using Vodo.Server.Controllers;
using Xunit;

namespace Vodo.UnitTests.Controllers
{
    public class JobObjectsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<JobObjectsController>> _loggerMock;
        private readonly JobObjectsController _controller;

        public JobObjectsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<JobObjectsController>>();
            _controller = new JobObjectsController(_mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetList_ReturnsOk_WithJobObjects()
        {
            var items = new List<JobObject> { new JobObject { Name = "J1" }, new JobObject { Name = "J2" } };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetJobObjectsQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync((IEnumerable<JobObject>)items);

            var result = await _controller.GetList();

            var action = Assert.IsType<ActionResult<IEnumerable<JobObject>>>(result);
            var ok = Assert.IsType<OkObjectResult>(action.Result);
            Assert.Equal(items, ok.Value);
        }

        [Fact]
        public async Task Create_ReturnsCreated_OnSuccess()
        {
            var newId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateJobObjectCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(newId);

            var command = new CreateJobObjectCommand { Name = "New" };
            var result = await _controller.Create(command);

            var action = Assert.IsType<ActionResult<Guid>>(result);
            var created = Assert.IsType<CreatedAtActionResult>(action.Result);
            Assert.Equal(newId, created.Value);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_IfIdMismatch()
        {
            var cmd = new UpdateJobObjectCommand { Id = Guid.NewGuid(), Name = "X" };
            var result = await _controller.Update(Guid.NewGuid(), cmd);

            var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("Id in route and body do not match", bad.Value.ToString());
        }

        [Fact]
        public async Task Update_ReturnsOk_OnSuccess()
        {
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateJobObjectCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(id);

            var cmd = new UpdateJobObjectCommand { Id = id, Name = "Updated" };
            var result = await _controller.Update(id, cmd);

            var action = Assert.IsType<ActionResult<Guid>>(result);
            var ok = Assert.IsType<OkObjectResult>(action.Result);
            Assert.Equal(id, ok.Value);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenHandlerThrowsKeyNotFound()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateJobObjectCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new KeyNotFoundException());

            var id = Guid.NewGuid();
            var cmd = new UpdateJobObjectCommand { Id = id, Name = "X" };
            var result = await _controller.Update(id, cmd);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_OnSuccess()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteJobObjectCommand>(), It.IsAny<CancellationToken>()))
                         .Returns(Task.FromResult(Unit.Value));

            var id = Guid.NewGuid();
            var result = await _controller.Delete(id);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenHandlerThrowsKeyNotFound()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteJobObjectCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.Delete(Guid.NewGuid());

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}