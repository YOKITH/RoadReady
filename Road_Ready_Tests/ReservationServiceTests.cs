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
    public class ReservationServiceTests
    {
        private Mock<IReservationRepository> _reservationRepositoryMock;

        private Mock<ICarRepository> _carRepositoryMock;

        private Mock<IMapper> _mapperMock;

        private Mock<ILogger<ReservationService>> _loggerMock;

        private ReservationService _reservationService;

        [SetUp]
        public void Setup()
        {
            _reservationRepositoryMock = new Mock<IReservationRepository>();

            _carRepositoryMock = new Mock<ICarRepository>();

            _mapperMock = new Mock<IMapper>();

            _loggerMock = new Mock<ILogger<ReservationService>>();

            _reservationService = new ReservationService(
                    _reservationRepositoryMock.Object,
                    _carRepositoryMock.Object,_mapperMock.Object,_loggerMock.Object);
        }

        [Test]
        public async Task GetAllReservationsAsync_Should_Return_All_Reservations()
        { 

            var reservations = new List<Reservation>
                {
                    new Reservation
                    {
                        ReservationId = 1
                    }
                };

            _reservationRepositoryMock.Setup(x =>
                    x.GetAllReservationsAsync()).ReturnsAsync(reservations);


            var result = await _reservationService.GetAllReservationsAsync();

            // Assert

            Assert.That(result.Count(),Is.EqualTo(1));
        }

        [Test]
        public void
        GetAllReservationsAsync_Should_Throw_Exception()
        {
            // Arrange

            _reservationRepositoryMock.Setup(x =>
                    x.GetAllReservationsAsync()).ThrowsAsync(new Exception());

            // Act + Assert

            Assert.ThrowsAsync<Exception>( async () =>await _reservationService
                        .GetAllReservationsAsync());
        }

        [Test]
        public async Task GetReservationByIdAsync_Should_Return_Reservation()
        {
            // Arrange

            var reservation =
                new Reservation
                {
                    ReservationId = 1
                };

            _reservationRepositoryMock
                .Setup(x =>
                    x.GetReservationByIdAsync(1))
                .ReturnsAsync(reservation);

            

            var result = await _reservationService
                    .GetReservationByIdAsync(1);

            Assert.That(result,Is.Not.Null);

            Assert.That(result!.ReservationId,Is.EqualTo(1));
        }

        [Test]
        public void GetReservationByIdAsync_Should_Throw_NotFoundException()
        {
            // Arrange

            _reservationRepositoryMock
                .Setup(x =>
                    x.GetReservationByIdAsync(1)).ReturnsAsync((Reservation?)null);

            // Act + Assert

            Assert.ThrowsAsync<KeyNotFoundException>(
                async () => await _reservationService.GetReservationByIdAsync(1));
        }

        [Test]
        public async Task GetReservationsByUserIdAsync_Should_Return_Reservations()
        {
            // Arrange

            var reservations =
                new List<Reservation>
                {
                    new Reservation
                    {
                        UserId = 1
                    }
                };

            _reservationRepositoryMock
                .Setup(x =>
                    x.GetReservationsByUserIdAsync(1))
                .ReturnsAsync(reservations);

            // Act

            var result =
                await _reservationService
                    .GetReservationsByUserIdAsync(1);

            // Assert

            Assert.That(
                result.Count(),
                Is.EqualTo(1));
        }

        [Test]
        public void
        GetReservationsByUserIdAsync_Should_Throw_Exception()
        {
            // Arrange

            _reservationRepositoryMock
                .Setup(x =>
                    x.GetReservationsByUserIdAsync(1))
                .ThrowsAsync(new Exception());

            // Act + Assert

            Assert.ThrowsAsync<Exception>(
                async () =>
                    await _reservationService
                        .GetReservationsByUserIdAsync(1));
        }

        [Test]
        public async Task
        CreateReservationAsync_Should_Create_Reservation()
        {
            // Arrange

            var dto =
                new Reservationdto
                {
                    UserId = 1,
                    CarId = 1,
                    PickupDate = DateTime.UtcNow.AddDays(1),
                    DropoffDate = DateTime.UtcNow.AddDays(3)
                };

            var car =
                new Car
                {
                    CarId = 1,
                    IsAvailable = true,
                    PricePerDay = 1000
                };

            var reservation =
                new Reservation();

            var response =
                new ReservationResponseDto
                {
                    ReservationId = 1
                };

            _carRepositoryMock
                .Setup(x =>
                    x.GetCarByIdAsync(1))
                .ReturnsAsync(car);

            _reservationRepositoryMock
                .Setup(x =>
                    x.IsCarBookedAsync(
                        dto.CarId,
                        dto.PickupDate,
                        dto.DropoffDate))
                .ReturnsAsync(false);

            _mapperMock
                .Setup(x =>
                    x.Map<Reservation>(dto))
                .Returns(reservation);

            _mapperMock
                .Setup(x =>
                    x.Map<ReservationResponseDto>(
                        It.IsAny<Reservation>()))
                .Returns(response);

            _reservationRepositoryMock
                .Setup(x =>
                    x.SaveChangesAsync())
                .ReturnsAsync(true);

            // Act

            var result =
                await _reservationService
                    .CreateReservationAsync(dto);

            // Assert

            Assert.That(
                result,
                Is.Not.Null);

            Assert.That(
                result.ReservationId,
                Is.EqualTo(1));

            _reservationRepositoryMock.Verify(
                x => x.AddReservationAsync(
                    It.IsAny<Reservation>()),
                Times.Once);
        }

        [Test]
        public void
        CreateReservationAsync_Should_Throw_When_Car_Not_Found()
        {
            // Arrange

            var dto =
                new Reservationdto
                {
                    CarId = 1
                };

            _carRepositoryMock
                .Setup(x =>
                    x.GetCarByIdAsync(1))
                .ReturnsAsync((Car?)null);

            // Act + Assert

            Assert.ThrowsAsync<KeyNotFoundException>(
                async () =>
                    await _reservationService
                        .CreateReservationAsync(dto));
        }

        [Test]
        public void
        CreateReservationAsync_Should_Throw_When_Car_Unavailable()
        {
            // Arrange

            var dto =
                new Reservationdto
                {
                    CarId = 1
                };

            var car =
                new Car
                {
                    CarId = 1,
                    IsAvailable = false
                };

            _carRepositoryMock
                .Setup(x =>
                    x.GetCarByIdAsync(1))
                .ReturnsAsync(car);

            // Act + Assert

            Assert.ThrowsAsync<InvalidOperationException>(
                async () =>
                    await _reservationService
                        .CreateReservationAsync(dto));
        }

        [Test]
        public void
        CreateReservationAsync_Should_Throw_When_PickupDate_Is_In_Past()
        {
            // Arrange

            var dto =
                new Reservationdto
                {
                    CarId = 1,
                    PickupDate = DateTime.UtcNow.AddDays(-1),
                    DropoffDate = DateTime.UtcNow.AddDays(1)
                };

            var car =
                new Car
                {
                    CarId = 1,
                    IsAvailable = true
                };

            _carRepositoryMock
                .Setup(x =>
                    x.GetCarByIdAsync(1))
                .ReturnsAsync(car);

            // Act + Assert

            Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await _reservationService
                        .CreateReservationAsync(dto));
        }

        [Test]
        public void
        CreateReservationAsync_Should_Throw_When_DropoffDate_Is_Invalid()
        {
            // Arrange

            var dto =
                new Reservationdto
                {
                    CarId = 1,
                    PickupDate = DateTime.UtcNow.AddDays(2),
                    DropoffDate = DateTime.UtcNow.AddDays(1)
                };

            var car =
                new Car
                {
                    CarId = 1,
                    IsAvailable = true
                };

            _carRepositoryMock
                .Setup(x =>
                    x.GetCarByIdAsync(1))
                .ReturnsAsync(car);

            // Act + Assert

            Assert.ThrowsAsync<ArgumentException>(
                async () =>
                    await _reservationService
                        .CreateReservationAsync(dto));
        }

        [Test]
        public void
        CreateReservationAsync_Should_Throw_When_Car_Already_Booked()
        {
            // Arrange

            var dto =
                new Reservationdto
                {
                    CarId = 1,
                    PickupDate = DateTime.UtcNow.AddDays(1),
                    DropoffDate = DateTime.UtcNow.AddDays(3)
                };

            var car =
                new Car
                {
                    CarId = 1,
                    IsAvailable = true
                };

            _carRepositoryMock
                .Setup(x =>
                    x.GetCarByIdAsync(1))
                .ReturnsAsync(car);

            _reservationRepositoryMock
                .Setup(x =>
                    x.IsCarBookedAsync(
                        dto.CarId,
                        dto.PickupDate,
                        dto.DropoffDate))
                .ReturnsAsync(true);

            // Act + Assert

            Assert.ThrowsAsync<InvalidOperationException>(
                async () =>
                    await _reservationService
                        .CreateReservationAsync(dto));
        }

        [Test]
        public void
        CreateReservationAsync_Should_Throw_When_Save_Fails()
        {
            // Arrange

            var dto =
                new Reservationdto
                {
                    UserId = 1,
                    CarId = 1,
                    PickupDate = DateTime.UtcNow.AddDays(1),
                    DropoffDate = DateTime.UtcNow.AddDays(3)
                };

            var car =
                new Car
                {
                    CarId = 1,
                    IsAvailable = true,
                    PricePerDay = 1000
                };

            var reservation = new Reservation();

            _carRepositoryMock
                .Setup(x =>
                    x.GetCarByIdAsync(1))
                .ReturnsAsync(car);

            _reservationRepositoryMock
                .Setup(x =>
                    x.IsCarBookedAsync(
                        dto.CarId,
                        dto.PickupDate,
                        dto.DropoffDate))
                .ReturnsAsync(false);

            _mapperMock.Setup(x =>
                    x.Map<Reservation>(dto)).Returns(reservation);

            _reservationRepositoryMock
                .Setup(x =>
                    x.SaveChangesAsync()).ReturnsAsync(false);

            // Act + Assert

            Assert.ThrowsAsync<Exception>(
                async () => await _reservationService.CreateReservationAsync(dto));
        }

        [Test]
        public async Task CancelReservationAsync_Should_Cancel_Reservation()
        {
            // Arrange

            var reservation =
                new Reservation
                {
                    ReservationId = 1,
                    Status = "Pending"
                };

            _reservationRepositoryMock
                .Setup(x =>
                    x.GetReservationByIdAsync(1))
                .ReturnsAsync(reservation);

            _reservationRepositoryMock
                .Setup(x =>
                    x.SaveChangesAsync())
                .ReturnsAsync(true);

            // Act

            var result =
                await _reservationService
                    .CancelReservationAsync(1);

            // Assert

            Assert.That(
                result,
                Is.True);

            Assert.That(
                reservation.Status,
                Is.EqualTo("Cancelled"));
        }

        [Test]
        public void
        CancelReservationAsync_Should_Throw_When_Reservation_Not_Found()
        {
            // Arrange

            _reservationRepositoryMock
                .Setup(x =>
                    x.GetReservationByIdAsync(1))
                .ReturnsAsync((Reservation?)null);

            // Act + Assert

            Assert.ThrowsAsync<KeyNotFoundException>(
                async () =>
                    await _reservationService
                        .CancelReservationAsync(1));
        }

        [Test]
        public async Task
        GetPagedReservationsAsync_Should_Return_Paged_Reservations()
        {
            // Arrange

            var pagedReservations =
                new PagedResponse<Reservation>
                {
                    Data =
                        new List<Reservation>
                        {
                            new Reservation
                            {
                                ReservationId = 1
                            }
                        },
                    PageNumber = 1,
                    PageSize = 10,
                    TotalRecords = 1
                };

            _reservationRepositoryMock
                .Setup(x =>
                    x.GetPagedReservationsAsync(
                        It.IsAny<PaginationParams>()))
                .ReturnsAsync(pagedReservations);

            // Act

            var result =
                await _reservationService
                    .GetPagedReservationsAsync(
                        new PaginationParams());

            // Assert

            Assert.That(
                result.TotalRecords,
                Is.EqualTo(1));
        }
    }
}