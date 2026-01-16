using Moq;
using Xunit;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.Contacts;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Contacts;

namespace SITRA.Tests.Repositories.Contacts
{
    public class ContactRepositoryTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IContactRepository> _contactRepositoryMock;

        public ContactRepositoryTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _contactRepositoryMock = new Mock<IContactRepository>();
            _unitOfWorkMock.Setup(u => u.ContactRepository).Returns(_contactRepositoryMock.Object);
        }

        [Fact]
        public async Task GetById_ReturnsContact_WhenContactExists()
        {
            // Arrange
            var contactId = 1;
            var expectedContact = new Contact { Id = contactId };
            _contactRepositoryMock.Setup(repo => repo.GetByIdAsync(contactId)).ReturnsAsync(expectedContact);

            // Act
            var result = await _unitOfWorkMock.Object.ContactRepository.GetByIdAsync(contactId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(contactId, result?.Id);
        }

        [Fact]
        public async Task GetAll_ReturnsListOfContacts()
        {
            // Arrange
            var paginationDTO = new PaginationDTO();
            var expectedContacts = new List<Contact> { new Contact { Id = 1 }, new Contact { Id = 2 } };
            _contactRepositoryMock.Setup(repo => repo.GetAllAsync(paginationDTO)).ReturnsAsync(expectedContacts);

            // Act
            var result = await _unitOfWorkMock.Object.ContactRepository.GetAllAsync(paginationDTO);

            // Assert
            Assert.Equal(expectedContacts.Count, result.Count);
        }

        [Fact]
        public async Task Activate_CallsActivateMethod()
        {
            // Arrange
            var contactId = 1;

            // Act
            await _unitOfWorkMock.Object.ContactRepository.ActivateAsync(contactId);

            // Assert
            _contactRepositoryMock.Verify(repo => repo.ActivateAsync(contactId), Times.Once);
        }

        [Fact]
        public async Task Deactivate_CallsDeactivateMethod()
        {
            // Arrange
            var contactId = 1;

            // Act
            await _unitOfWorkMock.Object.ContactRepository.DeactivateAsync(contactId);

            // Assert
            _contactRepositoryMock.Verify(repo => repo.DeactivateAsync(contactId), Times.Once);
        }

        [Fact]
        public async Task Exists_ReturnsTrue_WhenContactExists()
        {
            // Arrange
            var contactId = 1;
            _contactRepositoryMock.Setup(repo => repo.ExistsAsync(contactId)).ReturnsAsync(true);

            // Act
            var result = await _unitOfWorkMock.Object.ContactRepository.ExistsAsync(contactId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Exists_ReturnsFalse_WhenContactDoesNotExist()
        {
            // Arrange
            var contactId = 1;
            _contactRepositoryMock.Setup(repo => repo.ExistsAsync(contactId)).ReturnsAsync(false);

            // Act
            var result = await _unitOfWorkMock.Object.ContactRepository.ExistsAsync(contactId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task Exists_WithCode_ReturnsTrue_WhenContactExists()
        {
            // Arrange
            var contactId = 1;
            var code = "code123";
            _contactRepositoryMock.Setup(repo => repo.Exists(contactId, code)).ReturnsAsync(true);

            // Act
            var result = await _unitOfWorkMock.Object.ContactRepository.Exists(contactId, code);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Exists_WithCode_ReturnsFalse_WhenContactDoesNotExist()
        {
            // Arrange
            var contactId = 1;
            var code = "code123";
            _contactRepositoryMock.Setup(repo => repo.Exists(contactId, code)).ReturnsAsync(false);

            // Act
            var result = await _unitOfWorkMock.Object.ContactRepository.Exists(contactId, code);

            // Assert
            Assert.False(result);
        }
    }
}
