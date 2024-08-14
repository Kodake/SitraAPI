using Moq;
using NUnit.Framework;
using BackEnd.DTOs;
using BackEnd.Entities.Contacts;
using BackEnd.Repositories.Contacts;

namespace BackEnd.Tests.Repositories.Contacts
{
    [TestFixture]
    public class ContactRepositoryTests
    {
        private Mock<IContactRepository> _mockContactRepository = null!;

        [SetUp]
        public void Setup()
        {
            _mockContactRepository = new Mock<IContactRepository>();
        }

        [Test]
        public async Task GetById_ReturnsContact_WhenContactExists()
        {
            // Arrange
            var contactId = 1;
            var expectedContact = new Contact { Id = contactId };
            _mockContactRepository.Setup(repo => repo.GetById(contactId)).ReturnsAsync(expectedContact);

            // Act
            var result = await _mockContactRepository.Object.GetById(contactId);

            // Assert
            Assert.That(result, Is.Not.Null); // Ensure result is not null
            Assert.That(contactId, Is.EqualTo(result?.Id)); // Compare IDs
        }

        [Test]
        public async Task GetAll_ReturnsListOfContacts()
        {
            // Arrange
            var paginationDTO = new PaginationDTO();
            var expectedContacts = new List<Contact> { new Contact { Id = 1 }, new Contact { Id = 2 } };
            _mockContactRepository.Setup(repo => repo.GetAll(paginationDTO)).ReturnsAsync(expectedContacts);

            // Act
            var result = await _mockContactRepository.Object.GetAll(paginationDTO);

            // Assert
            Assert.That(expectedContacts.Count, Is.EqualTo(result.Count));
        }

        [Test]
        public async Task Create_ReturnsContactId_WhenContactIsCreated()
        {
            // Arrange
            var contact = new Contact();
            var expectedId = 1;
            _mockContactRepository.Setup(repo => repo.Create(contact)).ReturnsAsync(expectedId);

            // Act
            var result = await _mockContactRepository.Object.Create(contact);

            // Assert
            Assert.That(expectedId, Is.EqualTo(result));
        }

        [Test]
        public async Task Update_CallsUpdateMethod()
        {
            // Arrange
            var contact = new Contact();

            // Act
            await _mockContactRepository.Object.Update(contact);

            // Assert
            _mockContactRepository.Verify(repo => repo.Update(contact), Times.Once);
        }

        [Test]
        public async Task Activate_CallsActivateMethod()
        {
            // Arrange
            var contactId = 1;

            // Act
            await _mockContactRepository.Object.Activate(contactId);

            // Assert
            _mockContactRepository.Verify(repo => repo.Activate(contactId), Times.Once);
        }

        [Test]
        public async Task Deactivate_CallsDeactivateMethod()
        {
            // Arrange
            var contactId = 1;

            // Act
            await _mockContactRepository.Object.Deactivate(contactId);

            // Assert
            _mockContactRepository.Verify(repo => repo.Deactivate(contactId), Times.Once);
        }

        [Test]
        public async Task Exists_ReturnsTrue_WhenContactExists()
        {
            // Arrange
            var contactId = 1;
            _mockContactRepository.Setup(repo => repo.Exists(contactId)).ReturnsAsync(true);

            // Act
            var result = await _mockContactRepository.Object.Exists(contactId);

            // Assert
            Assert.That(result);
        }

        [Test]
        public async Task Exists_ReturnsFalse_WhenContactDoesNotExist()
        {
            // Arrange
            var contactId = 1;
            _mockContactRepository.Setup(repo => repo.Exists(contactId)).ReturnsAsync(false);

            // Act
            var result = await _mockContactRepository.Object.Exists(contactId);

            // Assert
            Assert.That(false, Is.EqualTo(result));
        }

        [Test]
        public async Task Exists_WithCode_ReturnsTrue_WhenContactExists()
        {
            // Arrange
            var contactId = 1;
            var code = "code123";
            _mockContactRepository.Setup(repo => repo.Exists(contactId, code)).ReturnsAsync(true);

            // Act
            var result = await _mockContactRepository.Object.Exists(contactId, code);

            // Assert
            Assert.That(true, Is.EqualTo(result));
        }

        [Test]
        public async Task Exists_WithCode_ReturnsFalse_WhenContactDoesNotExist()
        {
            // Arrange
            var contactId = 1;
            var code = "code123";
            _mockContactRepository.Setup(repo => repo.Exists(contactId, code)).ReturnsAsync(false);

            // Act
            var result = await _mockContactRepository.Object.Exists(contactId, code);

            // Assert
            Assert.That(false, Is.EqualTo(result));
        }
    }
}
