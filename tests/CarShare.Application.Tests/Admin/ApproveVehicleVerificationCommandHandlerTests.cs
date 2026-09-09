using CarShare.Application.Common.Interfaces;
using CarShare.Application.Features.Admin.Commands.ApproveVehicleVerification;
using CarShare.Domain.Entities;
using CarShare.Domain.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace CarShare.Application.Tests.Admin;

public sealed class ApproveVehicleVerificationCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldApproveVehicle_WhenVehicleIsPending()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();

        var vehicle = new Vehicle
        {
            Id = vehicleId,
            Brand = "Ford",
            Model = "Focus",
            VerificationStatus = VerificationStatus.Pending,
            RegistrationDocumentUrl =
                "https://example.com/registration.pdf"
        };

        var vehicleRepository =
            new Mock<IRepository<Vehicle>>();

        var unitOfWork =
            new Mock<IUnitOfWork>();

        vehicleRepository
            .Setup(repository => repository.Query())
            .Returns(new List<Vehicle>
            {
                vehicle
            }.AsQueryable());

        unitOfWork
            .Setup(unit => unit.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler =
            new ApproveVehicleVerificationCommandHandler(
                vehicleRepository.Object,
                unitOfWork.Object);

        var command =
            new ApproveVehicleVerificationCommand(vehicleId);

        // Act
        await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        vehicle.VerificationStatus
            .Should()
            .Be(VerificationStatus.Approved);

        unitOfWork.Verify(
            unit => unit.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenVehicleDoesNotExist()
    {
        // Arrange
        var vehicleRepository =
            new Mock<IRepository<Vehicle>>();

        var unitOfWork =
            new Mock<IUnitOfWork>();

        vehicleRepository
            .Setup(repository => repository.Query())
            .Returns(new List<Vehicle>().AsQueryable());

        var handler =
            new ApproveVehicleVerificationCommandHandler(
                vehicleRepository.Object,
                unitOfWork.Object);

        var command =
            new ApproveVehicleVerificationCommand(Guid.NewGuid());

        // Act
        var action = async () =>
            await handler.Handle(
                command,
                CancellationToken.None);

        // Assert
        await action
            .Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage("Vozilo nije pronađeno.");

        unitOfWork.Verify(
            unit => unit.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}