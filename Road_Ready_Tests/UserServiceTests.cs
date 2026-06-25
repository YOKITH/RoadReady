using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RoadReady.API.DTOs;
using RoadReady.API.Models;
using RoadReady.API.Pagination;
using RoadReady.API.Repositories.Interfaces;
using RoadReady.API.Services;

namespace RoadReady.Tests.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUserRepository>
            _userRepositoryMock;

        private Mock<ILogger<UserService>>
            _loggerMock;

        private Mock<IMapper>
            _mapperMock;

        private UserService
            _userService;

        [SetUp]
        public void Setup()
        {
            _userRepositoryMock =
                new Mock<IUserRepository>();

            _loggerMock =
                new Mock<ILogger<UserService>>();

            _mapperMock =
                new Mock<IMapper>();

            _userService =
                new UserService(
                    _userRepositoryMock.Object,
                    _loggerMock.Object,
                    _mapperMock.Object);
        }

        [Test]
        public async Task GetAllUsersAsync_Should_Return_All_Users()
        {
            // Arrange

            var users =
                new List<User>
                {
                    new User
                    {
                        UserId = 1,
                        FirstName = "Suresh"
                    }
                };

            _userRepositoryMock
                .Setup(x => x.GetAllUsersAsync())
                .ReturnsAsync(users);

            // Act

            var result =
                await _userService.GetAllUsersAsync();

            // Assert

            Assert.That(
                result.Count(),
                Is.EqualTo(1));
        }

        [Test]
        public void GetAllUsersAsync_Should_Throw_Exception()
        {
            // Arrange

            _userRepositoryMock
                .Setup(x => x.GetAllUsersAsync())
                .ThrowsAsync(new Exception());

            // Act + Assert

            Assert.ThrowsAsync<Exception>(
                async () =>
                    await _userService.GetAllUsersAsync());
        }

        [Test]
        public async Task GetUserByIdAsync_Should_Return_User()
        {
            // Arrange

            var user =
                new User
                {
                    UserId = 1,
                    FirstName = "Suresh"
                };

            _userRepositoryMock
                .Setup(x => x.GetUserByIdAsync(1))
                .ReturnsAsync(user);

            // Act

            var result =
                await _userService.GetUserByIdAsync(1);

            // Assert

            Assert.That(
                result,
                Is.Not.Null);

            Assert.That(
                result!.UserId,
                Is.EqualTo(1));
        }

        [Test]
        public void GetUserByIdAsync_Should_Throw_NotFoundException()
        {
            // Arrange

            _userRepositoryMock
                .Setup(x => x.GetUserByIdAsync(1))
                .ReturnsAsync((User?)null);

            // Act + Assert

            Assert.ThrowsAsync<KeyNotFoundException>(
                async () =>
                    await _userService.GetUserByIdAsync(1));
        }

        [Test]
        public async Task GetUserByEmailAsync_Should_Return_User()
        {
            // Arrange

            var user =
                new User
                {
                    UserId = 1,
                    Email = "suresh@gmail.com"
                };

            _userRepositoryMock
                .Setup(x =>
                    x.GetUserByEmailAsync(
                        "suresh@gmail.com"))
                .ReturnsAsync(user);

            // Act

            var result =
                await _userService.GetUserByEmailAsync(
                    "suresh@gmail.com");

            // Assert

            Assert.That(
                result,
                Is.Not.Null);

            Assert.That(
                result!.Email,
                Is.EqualTo(
                    "suresh@gmail.com"));
        }

        [Test]
        public void GetUserByEmailAsync_Should_Throw_NotFoundException()
        {
            // Arrange

            _userRepositoryMock
                .Setup(x =>
                    x.GetUserByEmailAsync(
                        "test@gmail.com"))
                .ReturnsAsync((User?)null);

            // Act + Assert

            Assert.ThrowsAsync<KeyNotFoundException>(
                async () =>
                    await _userService.GetUserByEmailAsync(
                        "test@gmail.com"));
        }

        [Test]
        public async Task EmailExistsAsync_Should_Return_True()
        {
            // Arrange

            _userRepositoryMock
                .Setup(x =>
                    x.EmailExistsAsync(
                        "suresh@gmail.com"))
                .ReturnsAsync(true);

            // Act

            var result =
                await _userService.EmailExistsAsync(
                    "suresh@gmail.com");

            // Assert

            Assert.That(
                result,
                Is.True);
        }

        [Test]
        public async Task EmailExistsAsync_Should_Return_False()
        {
            // Arrange

            _userRepositoryMock
                .Setup(x =>
                    x.EmailExistsAsync(
                        "test@gmail.com"))
                .ReturnsAsync(false);

            // Act

            var result =
                await _userService.EmailExistsAsync(
                    "test@gmail.com");

            // Assert

            Assert.That(
                result,
                Is.False);
        }

        [Test]
        public async Task RegisterUserAsync_Should_Register_User()
        {
            // Arrange

            var dto =
                new RegisterDto
                {
                    FirstName = "Suresh",
                    LastName = "Krishna",
                    Email = "suresh@gmail.com",
                    Password = "Password@123"
                };

            var user =
                new User();

            _userRepositoryMock
                .Setup(x =>
                    x.GetUserByEmailAsync(
                        dto.Email))
                .ReturnsAsync((User?)null);

            _mapperMock
                .Setup(x =>
                    x.Map<User>(dto))
                .Returns(user);

            _userRepositoryMock
                .Setup(x =>
                    x.SaveChangesAsync())
                .ReturnsAsync(true);

            // Act

            var result =
                await _userService.RegisterUserAsync(dto);

            // Assert

            Assert.That(
                result,
                Is.True);

            _userRepositoryMock.Verify(
                x => x.AddUserAsync(
                    It.IsAny<User>()),
                Times.Once);
        }

        [Test]
        public void RegisterUserAsync_Should_Throw_When_Email_Already_Exists()
        {
            // Arrange

            var dto =
                new RegisterDto
                {
                    Email = "suresh@gmail.com"
                };

            _userRepositoryMock
                .Setup(x =>
                    x.GetUserByEmailAsync(
                        dto.Email))
                .ReturnsAsync(new User());

            // Act + Assert

            Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await _userService.RegisterUserAsync(dto));
        }

        [Test]
        public async Task UpdateUserAsync_Should_Update_User()
        {
            // Arrange

            var existingUser =
                new User
                {
                    UserId = 1,
                    FirstName = "Old"
                };

            var updatedUser =
                new User
                {
                    FirstName = "New",
                    LastName = "User",
                    PhoneNumber = "9876543210"
                };

            _userRepositoryMock
                .Setup(x => x.GetUserByIdAsync(1))
                .ReturnsAsync(existingUser);

            _userRepositoryMock
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(true);

            // Act

            var result =
                await _userService.UpdateUserAsync(
                    1,
                    updatedUser);

            // Assert

            Assert.That(
                result,
                Is.True);

            Assert.That(
                existingUser.FirstName,
                Is.EqualTo("New"));
        }

        [Test]
        public void UpdateUserAsync_Should_Throw_When_User_Not_Found()
        {
            // Arrange

            _userRepositoryMock
                .Setup(x => x.GetUserByIdAsync(1))
                .ReturnsAsync((User?)null);

            // Act + Assert

            Assert.ThrowsAsync<KeyNotFoundException>(
                async () =>
                    await _userService.UpdateUserAsync(
                        1,
                        new User()));
        }

        [Test]
        public async Task DeleteUserAsync_Should_Delete_User()
        {
            // Arrange

            var user =
                new User
                {
                    UserId = 1
                };

            _userRepositoryMock
                .Setup(x => x.GetUserByIdAsync(1))
                .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(true);

            // Act

            var result =
                await _userService.DeleteUserAsync(1);

            // Assert

            Assert.That(
                result,
                Is.True);

            _userRepositoryMock.Verify(
                x => x.DeleteUserAsync(user),
                Times.Once);
        }

        [Test]
        public void DeleteUserAsync_Should_Throw_When_User_Not_Found()
        {
            // Arrange

            _userRepositoryMock
                .Setup(x => x.GetUserByIdAsync(1))
                .ReturnsAsync((User?)null);

            // Act + Assert

            Assert.ThrowsAsync<KeyNotFoundException>(
                async () =>
                    await _userService.DeleteUserAsync(1));
        }

        [Test]
        public async Task GetPagedUsersAsync_Should_Return_Paged_Users()
        {
            // Arrange

            var pagedUsers =
                new PagedResponse<User>
                {
                    Data =
                        new List<User>
                        {
                            new User
                            {
                                UserId = 1,
                                FirstName = "Suresh"
                            }
                        },
                    PageNumber = 1,
                    PageSize = 10,
                    TotalRecords = 1
                };

            _userRepositoryMock
                .Setup(x =>
                    x.GetPagedUsersAsync(
                        It.IsAny<PaginationParams>()))
                .ReturnsAsync(pagedUsers);

            // Act

            var result =
                await _userService.GetPagedUsersAsync(
                    new PaginationParams());

            // Assert

            Assert.That(
                result.TotalRecords,
                Is.EqualTo(1));
        }
    }
}