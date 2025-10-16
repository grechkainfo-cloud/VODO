using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Vodo.Models;
using Vodo.Application.Requests.Contractors.CreateContractor;
using Vodo.Application.Requests.Contractors.DeleteContractor;
using Vodo.Application.Requests.Contractors.GetContractors;
using Vodo.Application.Requests.Contractors.UpdateContractor;
using Vodo.Server.Controllers;
using Xunit;

namespace Vodo.UnitTests.Server.Controllers
{
    /// <summary>
    /// Тесты для контроллера ContractorsController.
    /// Проверяют корректность работы методов контроллера и взаимодействие с MediatR.
    /// </summary>
    public class ContractorsControllerTests
    {
        private ContractorsController CreateController(Mock<IMediator> mediatorMock, Mock<ILogger<ContractorsController>>? loggerMock = null)
        {
            return new ContractorsController(
                mediatorMock.Object,
                loggerMock?.Object ?? new Mock<ILogger<ContractorsController>>().Object
            );
        }

        [Fact]
        public async Task GetList_ReturnsOk_WithContractors()
        {
            // Arrange
            var contractors = new List<Contractor>
            {
                new Contractor { Id = Guid.NewGuid(), Name = "A", Inn = "1", Payload = null },
                new Contractor { Id = Guid.NewGuid(), Name = "B", Inn = "2", Payload = null }
            };
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<GetContractorsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(contractors);

            var loggerMock = new Mock<ILogger<ContractorsController>>();
            var controller = CreateController(mediatorMock, loggerMock);

            // Act
            var result = await controller.GetList();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(contractors, okResult.Value);
        }

        [Fact]
        public async Task Create_ReturnsCreated_WithId()
        {
            // Arrange
            var contractorId = Guid.NewGuid();
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<CreateContractorCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(contractorId);

            var loggerMock = new Mock<ILogger<ContractorsController>>();
            var controller = CreateController(mediatorMock, loggerMock);
            var command = new CreateContractorCommand { Name = "Test", Inn = "123", Payload = null };

            // Act
            var result = await controller.Create(command);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(contractorId, createdResult.Value);
        }

        [Fact]
        public async Task Update_ReturnsOk_WithId_When_IdsMatch()
        {
            // Arrange
            var contractorId = Guid.NewGuid();
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<UpdateContractorCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(contractorId);

            var loggerMock = new Mock<ILogger<ContractorsController>>();
            var controller = CreateController(mediatorMock, loggerMock);
            var command = new UpdateContractorCommand { Id = contractorId, Name = "Updated", Inn = "456", Payload = null };

            // Act
            var result = await controller.Update(contractorId, command);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(contractorId, okResult.Value);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_When_IdsDoNotMatch()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();
            var loggerMock = new Mock<ILogger<ContractorsController>>();
            var controller = CreateController(mediatorMock, loggerMock);
            var command = new UpdateContractorCommand { Id = Guid.NewGuid(), Name = "Updated", Inn = "456", Payload = null };

            // Act
            var result = await controller.Update(Guid.NewGuid(), command);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<DeleteContractorCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var loggerMock = new Mock<ILogger<ContractorsController>>();
            var controller = CreateController(mediatorMock, loggerMock);

            // Act
            var result = await controller.Delete(Guid.NewGuid());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}