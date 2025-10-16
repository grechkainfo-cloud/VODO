using Microsoft.EntityFrameworkCore;
using Vodo.Application.Requests.Contractors.DeleteContractor;
using Vodo.DAL.Context;
using Vodo.Models;
using Microsoft.EntityFrameworkCore.InMemory;

namespace Vodo.UnitTests.Application.Requests.Contractors
{
    public class DeleteContractorCommandHandlerTests
    {
        private VodoContext CreateContextWithContractor(Guid contractorId)
        {
            var options = new DbContextOptionsBuilder<VodoContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new VodoContext(options);
            context.Contractors.Add(new Contractor
            {
                Id = contractorId,
                Name = "Test Contractor",
                Inn = "123456789012",
                Payload = new Dictionary<string, object> { { "email", "test@example.com" } }
            });
            context.SaveChanges();
            return context;
        }

        /// <summary>
        /// Проверяет, что если подрядчик с указанным Id существует в базе данных,
        /// то после выполнения команды удаления он будет удалён.
        /// Ожидаемое поведение: подрядчик отсутствует в базе после вызова обработчика.
        /// </summary>
        [Fact]
        public async Task Handle_Removes_Contractor_When_Exists()
        {
            // Arrange
            var contractorId = Guid.NewGuid();
            var context = CreateContextWithContractor(contractorId);
            var handler = new DeleteContractorCommandHandler(context);
            var command = new DeleteContractorCommand { Id = contractorId };

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            var contractor = await context.Contractors.FindAsync(contractorId);
            Assert.Null(contractor);
        }

        /// <summary>
        /// Проверяет, что если подрядчик с указанным Id отсутствует в базе данных,
        /// то при попытке удаления будет выброшено исключение.
        /// Ожидаемое поведение: выбрасывается Exception, так как подрядчик не найден.
        /// </summary>
        [Fact]
        public async Task Handle_Throws_When_Contractor_NotFound()
        {
            // Arrange
            var context = CreateContextWithContractor(Guid.NewGuid());
            var handler = new DeleteContractorCommandHandler(context);
            var nonExistentId = Guid.NewGuid();
            var command = new DeleteContractorCommand { Id = nonExistentId };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}