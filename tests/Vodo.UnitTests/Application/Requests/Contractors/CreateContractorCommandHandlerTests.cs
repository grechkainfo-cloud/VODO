using Microsoft.EntityFrameworkCore;
using Vodo.Application.Requests.Contractors.CreateContractor;
using Vodo.DAL.Context;

namespace Vodo.UnitTests.Application.Requests.Contractors
{
    /// <summary>
    /// ����� ������ ��� ����������� <see cref="CreateContractorCommandHandler"/>,
    /// ����������� ������������ �������� ���������� � ���������� ���� ��� ����� � ���� ������.
    /// </summary>
    public class CreateContractorCommandHandlerTests
    {
        /// <summary>
        /// ������ ����� ��������� <see cref="VodoContext"/> � �������������� InMemoryDatabase ��� �������� ������.
        /// </summary>
        /// <returns>��������� <see cref="VodoContext"/>.</returns>
        private VodoContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<VodoContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new VodoContext(options);
        }

        /// <summary>
        /// ���������, ��� ���������� <see cref="CreateContractorCommandHandler"/> ��������� ������ ����������
        /// � ����������� ���� �����, � ��� ��� ���� ����������� � ���� ������.
        /// ��������� ���������: ��������� ��������, ��� ���� ������������� ������ �������.
        /// </summary>
        [Fact]
        public async Task Handle_Creates_Contractor_With_All_Fields()
        {
            // Arrange
            var context = CreateContext();
            var handler = new CreateContractorCommandHandler(context);
            var payload = new Dictionary<string, object> { { "email", "test@example.com" }, { "phone", "123456" } };
            var command = new CreateContractorCommand
            {
                Name = "Test Contractor",
                Inn = "123456789012",
                Payload = payload
            };

            // Act
            var contractorId = await handler.Handle(command, CancellationToken.None);

            // Assert
            var contractor = await context.Contractors.FindAsync(contractorId);
            Assert.NotNull(contractor);
            Assert.Equal("Test Contractor", contractor!.Name);
            Assert.Equal("123456789012", contractor.Inn);
            Assert.NotNull(contractor.Payload);
            Assert.Equal("test@example.com", contractor.Payload!["email"]);
            Assert.Equal("123456", contractor.Payload!["phone"]);
        }
    }
}