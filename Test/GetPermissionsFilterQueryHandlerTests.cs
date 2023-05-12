using Application.Permissions.Querys;
using Domain.Interface.Permissions;
using Domain.Permissions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[TestFixture]
public class GetPermissionsFilterQueryHandlerTests
{
    private Mock<IPermissionRepository> _mockPermissionRepository;
    private GetPermissionsFilterQueryHandler _handler;

    [SetUp]
    public void Setup()
    {
        _mockPermissionRepository = new Mock<IPermissionRepository>();
        _handler = new GetPermissionsFilterQueryHandler(_mockPermissionRepository.Object);
    }

    [Test]
    public async Task Handle_ValidId_ReturnsPermission()
    {
        // Arrange
        var permission = new Permission { Id = 1 };
        _mockPermissionRepository.Setup(x => x.GetPermissionByIdAsync(1))
                                 .ReturnsAsync(permission);

        var query = new GetPermissionsFilterQuery(1) {  };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(permission));
    }

    [Test]
    public async Task Handle_InvalidId_ReturnsNull()
    {
        // Arrange
        _mockPermissionRepository.Setup(x => x.GetPermissionByIdAsync(1))
                                 .ReturnsAsync((Permission)null);

        var query = new GetPermissionsFilterQuery (1){  };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsNull(result);
    }


    [Test]
    public async Task Handle_ValidId_ReturnsPermission2()
    {
        // Arrange
        int id = new Random().Next(1, 100);
        var permission = new Permission
        {
            Id = id,
            EmployeeForename = "John",
            EmployeSurname = "Doe",
            PermissionTypeId = 1,
            PermissionDate = DateTime.UtcNow
        };
        _mockPermissionRepository.Setup(x => x.GetPermissionByIdAsync(id))
                                 .ReturnsAsync(permission);

        var query = new GetPermissionsFilterQuery(id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Id, Is.EqualTo(permission.Id));
        Assert.That(result.EmployeeForename, Is.EqualTo(permission.EmployeeForename));
        Assert.That(result.EmployeSurname, Is.EqualTo(permission.EmployeSurname));
        Assert.That(result.PermissionTypeId, Is.EqualTo(permission.PermissionTypeId));
        Assert.That(result.PermissionDate, Is.EqualTo(permission.PermissionDate));
    }

    [Test]
    public async Task Handle_InvalidId_ReturnsNull2()
    {
        // Arrange
        int id = new Random().Next(1, 100);
        _mockPermissionRepository.Setup(x => x.GetPermissionByIdAsync(id))
                                 .ReturnsAsync((Permission)null);

        var query = new GetPermissionsFilterQuery(id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsNull(result);
    }
}
