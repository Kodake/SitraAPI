using BackEnd.DTOs;
using BackEnd.Entities.Contacts;
using BackEnd.Repositories;
using Moq;
using NUnit.Framework;

namespace BackEnd.Tests.Repositories.Contacts
{
    [TestFixture]
    public class ContactRepositoryTests
    {
        private Mock<IUnitOfWork> _unitOfWorkRepository = null!;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkRepository = new Mock<IUnitOfWork>();
        }

        [Test]
        public async Task GetById_ReturnsContact_WhenContactExists()
        {
            // Arrange
            var contactId = 1;
            var expectedContact = new Contact { Id = contactId };
            _unitOfWorkRepository.Setup(repo => repo.ContactRepository.GetByIdAsync(contactId)).ReturnsAsync(expectedContact);

            // Act
            var result = await _unitOfWorkRepository.Object.ContactRepository.GetByIdAsync(contactId);

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
            _unitOfWorkRepository.Setup(repo => repo.ContactRepository.GetAllAsync(paginationDTO)).ReturnsAsync(expectedContacts);

            // Act
            var result = await _unitOfWorkRepository.Object.ContactRepository.GetAllAsync(paginationDTO);

            // Assert
            Assert.That(expectedContacts.Count, Is.EqualTo(result.Count));
        }


        [Test]
        public async Task Activate_CallsActivateMethod()
        {
            // Arrange
            var contactId = 1;

            // Act
            await _unitOfWorkRepository.Object.ContactRepository.ActivateAsync(contactId);

            // Assert
            _unitOfWorkRepository.Verify(repo => repo.ContactRepository.ActivateAsync(contactId), Times.Once);
        }

        [Test]
        public async Task Deactivate_CallsDeactivateMethod()
        {
            // Arrange
            var contactId = 1;

            // Act
            await _unitOfWorkRepository.Object.ContactRepository.DeactivateAsync(contactId);

            // Assert
            _unitOfWorkRepository.Verify(repo => repo.ContactRepository.DeactivateAsync(contactId), Times.Once);
        }

        [Test]
        public async Task Exists_ReturnsTrue_WhenContactExists()
        {
            // Arrange
            var contactId = 1;
            _unitOfWorkRepository.Setup(repo => repo.ContactRepository.ExistsAsync(contactId)).ReturnsAsync(true);

            // Act
            var result = await _unitOfWorkRepository.Object.ContactRepository.ExistsAsync(contactId);

            // Assert
            Assert.That(result);
        }

        [Test]
        public async Task Exists_ReturnsFalse_WhenContactDoesNotExist()
        {
            // Arrange
            var contactId = 1;
            _unitOfWorkRepository.Setup(repo => repo.ContactRepository.ExistsAsync(contactId)).ReturnsAsync(false);

            // Act
            var result = await _unitOfWorkRepository.Object.ContactRepository.ExistsAsync(contactId);

            // Assert
            Assert.That(false, Is.EqualTo(result));
        }

        [Test]
        public async Task Exists_WithCode_ReturnsTrue_WhenContactExists()
        {
            // Arrange
            var contactId = 1;
            var code = "code123";
            _unitOfWorkRepository.Setup(repo => repo.ContactRepository.Exists(contactId, code)).ReturnsAsync(true);

            // Act
            var result = await _unitOfWorkRepository.Object.ContactRepository.Exists(contactId, code);

            // Assert
            Assert.That(true, Is.EqualTo(result));
        }

        [Test]
        public async Task Exists_WithCode_ReturnsFalse_WhenContactDoesNotExist()
        {
            // Arrange
            var contactId = 1;
            var code = "code123";
            _unitOfWorkRepository.Setup(repo => repo.ContactRepository.Exists(contactId, code)).ReturnsAsync(false);

            // Act
            var result = await _unitOfWorkRepository.Object.ContactRepository.Exists(contactId, code);

            // Assert
            Assert.That(false, Is.EqualTo(result));
        }
    }
}
