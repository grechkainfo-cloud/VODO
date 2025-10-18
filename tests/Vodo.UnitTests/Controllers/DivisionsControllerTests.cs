using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Vodo.Application.Requests.Divisions.CreateDivision;
using Vodo.Application.Requests.Divisions.DeleteDivision;
using Vodo.Application.Requests.Divisions.GetDivisions;
using Vodo.Application.Requests.Divisions.UpdateDivision;
using Vodo.Models;
using Vodo.Server.Controllers;
using Xunit;

namespace Vodo.UnitTests.Controllers
{
    public class DivisionsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<Microsoft.Extensions.Logging.ILogger<DivisionsController>> _loggerMock;
        private readonly DivisionsController _controller;

        public DivisionsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<DivisionsController>>();
            _controller = new DivisionsController(_mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetList_ReturnsOk_WithDivisions()
        {
            var divisions = new List<Division> { new Division { Name = "D1" }, new Division { Name = "D2" } };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetDivisionsQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync((IEnumerable<Division>)divisions);

            var result = await _controller.GetList();

            var ok = Assert.IsType<ActionResult<IEnumerable<Division>>>(result);
            var actionResult = Assert.IsType<OkObjectResult>(ok.Result);
            Assert.Equal(divisions, actionResult.Value);
        }

        [Fact]
        public async Task Create_ReturnsCreated_OnSuccess()
        {
            var newId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateDivisionCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(newId);

            var command = new CreateDivisionCommand { Name = "New" };
            var result = await _controller.Create(command);

            var action = Assert.IsType<ActionResult<Guid>>(result);
            var created = Assert.IsType<CreatedAtActionResult>(action.Result);
            Assert.Equal(newId, created.Value);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_OnValidationException()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateDivisionCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new ValidationException("bad"));

            var command = new CreateDivisionCommand { Name = "bad" };
            var result = await _controller.Create(command);

            var action = Assert.IsType<ActionResult<Guid>>(result);
            var bad = Assert.IsType<BadRequestObjectResult>(action.Result);
            Assert.Equal("bad", bad.Value);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_IfIdMismatch()
        {
            var cmd = new UpdateDivisionCommand { Id = Guid.NewGuid(), Name = "X" };
            var result = await _controller.Update(Guid.NewGuid(), cmd);

            var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("Id in route and body do not match", bad.Value.ToString());
        }

        [Fact]
        public async Task Update_ReturnsOk_OnSuccess()
        {
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateDivisionCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(id);

            var cmd = new UpdateDivisionCommand { Id = id, Name = "Updated" };
            var result = await _controller.Update(id, cmd);

            var action = Assert.IsType<ActionResult<Guid>>(result);
            var ok = Assert.IsType<OkObjectResult>(action.Result);
            Assert.Equal(id, ok.Value);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenHandlerThrowsKeyNotFound()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateDivisionCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new KeyNotFoundException());

            var id = Guid.NewGuid();
            var cmd = new UpdateDivisionCommand { Id = id, Name = "X" };
            var result = await _controller.Update(id, cmd);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_OnSuccess()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteDivisionCommand>(), It.IsAny<CancellationToken>()))
                         .Returns(Task.FromResult(Unit.Value));

            var id = Guid.NewGuid();
            var result = await _controller.Delete(id);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenHandlerThrowsKeyNotFound()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteDivisionCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.Delete(Guid.NewGuid());

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}