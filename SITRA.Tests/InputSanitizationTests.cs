using MediatR;
using Moq;
using SITRA.Core.Mediator.Behaviors;
using Xunit;

namespace SITRA.Tests
{
    public class InputSanitizationTests
    {
        public class TestCommand : IRequest<string>
        {
            public string Name { get; set; } = string.Empty;
            public string? Description { get; set; }
            public int Age { get; set; }
            public string ReadOnlyProp { get; } = "  readonly  ";
        }

        [Fact]
        public async Task Handle_TrimsStringProperties()
        {
            // Arrange
            var behavior = new InputSanitizationBehavior<TestCommand, string>();
            var command = new TestCommand
            {
                Name = "  John Doe  ",
                Description = "  Some Description  ",
                Age = 30
            };

            RequestHandlerDelegate<string> next = delegate { return Task.FromResult("Success"); };
            
            // Act
            await behavior.Handle(command, next, CancellationToken.None);

            // Assert
            Assert.Equal("John Doe", command.Name);
            Assert.Equal("Some Description", command.Description);
            Assert.Equal(30, command.Age);
        }

        [Fact]
        public async Task Handle_IgnoresNullStrings()
        {
            // Arrange
            var behavior = new InputSanitizationBehavior<TestCommand, string>();
            var command = new TestCommand
            {
                Name = "Test",
                Description = null
            };

            RequestHandlerDelegate<string> next = delegate { return Task.FromResult("Success"); };
            
            // Act
            await behavior.Handle(command, next, CancellationToken.None);

            // Assert
            Assert.Equal("Test", command.Name);
            Assert.Null(command.Description);
        }
        [Fact]
        public async Task Handle_RemovesHtmlTags()
        {
            // Arrange
            var behavior = new InputSanitizationBehavior<TestCommand, string>();
            var command = new TestCommand
            {
                Name = "<script>alert('xss')</script> John",
                Description = "<b>Bold</b> Text"
            };

            RequestHandlerDelegate<string> next = delegate { return Task.FromResult("Success"); };
            
            // Act
            await behavior.Handle(command, next, CancellationToken.None);

            // Assert
            Assert.Equal("alert('xss') John", command.Name); // Strips <script> and </script>, leaving "alert('xss') John"
            Assert.Equal("Bold Text", command.Description); // Strips <b> and </b>
        }
    }
}
