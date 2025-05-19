using Moq;
using Application.Commands;
using Domain.Entities;
using Application.Interfaces;
using Application.Handlers;

public class CreateTransactionHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnTransaction_WhenTransactionIsCreated()
    {
        Transaction createdTransaction = null!;
        var mockRepo = new Mock<ITransactionRepository>();
        mockRepo.Setup(r => r.CreateTransactionAsync(It.IsAny<Transaction>()))
                .Callback<Transaction>(t => createdTransaction = t)
                .ReturnsAsync((Transaction t) => t);

        var mockKafka = new Mock<IKafka>();

        var handler = new CreateTransactionHandler(mockRepo.Object, mockKafka.Object);

        var command = new CreateTransactionCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            100
        );

        var result = await handler.Handle(command);

        Assert.NotNull(createdTransaction);
        Assert.Equal(createdTransaction.Id, result);
        mockRepo.Verify(r => r.CreateTransactionAsync(It.IsAny<Transaction>()), Times.Once);
    }
}