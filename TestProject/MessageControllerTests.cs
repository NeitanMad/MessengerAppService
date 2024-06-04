using MessageDataBase.BD;
using MessageDataBase.Repository;
using MessageService.Controllers;
using MessageService.DTO;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace TestProject
{
    [TestFixture]
    public class MessageControllerTests
    {
        private MessageController _controller;
        private Mock<IMessageRepository> _mockMessageRepository;

        [SetUp]
        public void Setup()
        {
            _mockMessageRepository = new Mock<IMessageRepository>();
            _controller = new MessageController(null, _mockMessageRepository.Object);
        }

        [Test]
        public void GetAllMessages_ReturnsOkResult()
        {
            // Arrange
            var expectedMessages = new List<Message> {
                new Message {
                    Text = "Hello, world!", SenderName = "Neitan",
                    ReceiverName = "Neitan"
                },
                new Message {
                    Text = "Как дела?", SenderName = "Neitan",
                    ReceiverName = "Neitan"
                }
            };
            _mockMessageRepository.Setup(x => x.GetAllMessages(It.IsAny<string>())).Returns(expectedMessages);

            // Act
            var result = _controller.GetAllMessages("Neitan");

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void SendMessage_ReturnsOkResult()
        {
            // Arrange
            var id = Guid.NewGuid();
            var message = new MessageDTO { Text = "Hello, world!", SenderName = "Neitan", ReceiverName = "Alaska" };
            _mockMessageRepository.Setup(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(id);

            // Act
            var result = _controller.SendMessage(message) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(id.ToString(), result.Value.ToString());
        }
    }
}
