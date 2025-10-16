using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Vodo.Application.Requests.Contractors.UpdateContractor;
using Vodo.DAL.Context;
using Vodo.Models;
using Xunit;

namespace Vodo.UnitTests.Application.Requests.Contractors
{
    public class UpdateContractorCommandHandlerTests
    {
        private VodoContext CreateContextWithContractor(Guid contractorId, string name, string? inn, Dictionary<string, object>? payload)
        {
            var options = new DbContextOptionsBuilder<VodoContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new VodoContext(options);
            context.Contractors.Add(new Contractor
            {
                Id = contractorId,
                Name = name,
                Inn = inn,
                Payload = payload
            });
            context.SaveChanges();
            return context;
        }

        /// <summary>
        /// Проверяет, что обработчик обновляет все поля подрядчика, если подрядчик существует.
        /// Ожидаемое поведение: все поля обновляются согласно команде.
        /// </summary>
        [Fact]
        public async Task Handle_Updates_All_Fields_When_Contractor_Exists()
        {
            // Arrange
            var contractorId = Guid.NewGuid();
            var initialPayload = new Dictionary<string, object> { { "email", "old@example.com" } };
            var context = CreateContextWithContractor(contractorId, "Old Name", "111111111111", initialPayload);

            var handler = new UpdateContractorCommandHandler(context);
            var newPayload = new Dictionary<string, object> { { "email", "new@example.com" }, { "phone", "123456" } };
            var command = new UpdateContractorCommand
            {
                Id = contractorId,
                Name = "New Name",
                Inn = "222222222222",
                Payload = newPayload
            };

            // Act
            var resultId = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(contractorId, resultId);
            var updated = await context.Contractors.FindAsync(contractorId);
            Assert.NotNull(updated);
            Assert.Equal("New Name", updated!.Name);
            Assert.Equal("222222222222", updated.Inn);
            Assert.NotNull(updated.Payload);
            Assert.Equal("new@example.com", updated.Payload!["email"]);
            Assert.Equal("123456", updated.Payload!["phone"]);
        }

        /// <summary>
        /// Проверяет, что если подрядчик с указанным Id отсутствует, выбрасывается Exception.
        /// Ожидаемое поведение: выбрасывается Exception.
        /// </summary>
        [Fact]
        public async Task Handle_Throws_When_Contractor_NotFound()
        {
            // Arrange
            var context = CreateContextWithContractor(Guid.NewGuid(), "Name", "123", null);
            var handler = new UpdateContractorCommandHandler(context);
            var nonExistentId = Guid.NewGuid();
            var command = new UpdateContractorCommand
            {
                Id = nonExistentId,
                Name = "Any",
                Inn = "Any",
                Payload = new Dictionary<string, object> { { "email", "any@example.com" } }
            };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}