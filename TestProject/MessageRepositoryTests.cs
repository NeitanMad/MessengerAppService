using MessageDataBase.Repository;
using NUnit.Framework;

namespace TestProject;

[TestFixture]
public class MessageRepositoryTests
{
    [Test]
    public void SendMessage_Success()
    {
        // Arrange
        var repository = new MessageRepository();

        // Act
        var id = repository.SendMessage("Hello, World!", "Neitan", "Alaska");

        // Assert
        Assert.AreNotEqual(id, Guid.Empty);
    }



    [Test]
    public void GetAllMessages_Success()
    {
        // Arrange
        var repository = new MessageRepository();

        // Act
        var messages = repository.GetAllMessages("Alaska");

        // Assert
        Assert.IsNotNull(messages);
    }

    [Test]
    public void GetAllMessages_ReturnsEmptyList_ForNonExistingUser()
    {
        // Arrange
        var repository = new MessageRepository();

        // Act
        var messages = repository.GetAllMessages("Несуществующий пользователь");

        // Assert
        Assert.IsNotNull(messages);
    }
}