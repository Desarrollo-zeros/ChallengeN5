using Application.ElasticsearchServices;
using Application.Permissions.Commands;
using Domain.Interface.ElasticsearchServices;
using Domain.Interface.Permissions;
using Domain.Interface.UnitOfWorks;
using Domain.Permissions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[TestFixture]
public class RequestPermissionCommandHandlerTests
{

    private Mock<IPermissionRepository> _mockPermissionRepository;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private RequestPermissionCommandHandler _handler;
    private Mock<IElasticSearchApplication<Permission>> _elasticSearchApplication;

    [SetUp]
    public void Setup()
    {
        _mockPermissionRepository = new Mock<IPermissionRepository>();
        
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        _elasticSearchApplication = new Mock<IElasticSearchApplication<Permission>>();
        // Arrange
        var permission = new Permission { Id = 1, PermissionTypeId = 1 };
        var permissionType = new List<PermissionType>
        {
            new PermissionType
            {
                Id = 1,
                Description = "hola"
            },
            new PermissionType
            {
                Id = 2,
            }
        };
        _mockPermissionRepository.Setup(x => x.GetByIdAsync(permission.Id)).ReturnsAsync(permission);
        _mockUnitOfWork.Setup(x => x.PermissionRepository).Returns(_mockPermissionRepository.Object);
        _mockUnitOfWork.Setup(x => x.PermissionTypeRepository.GetByIdAsync(1)).ReturnsAsync(permissionType?.FirstOrDefault());
        _handler = new RequestPermissionCommandHandler(_mockPermissionRepository.Object, _elasticSearchApplication.Object, _mockUnitOfWork.Object);

    }

    [Test]
    public async Task Handle_WithValidCommand_ShouldReturnNewPermission()
    {
        // Arrange
        var command = new RequestPermissionCommand
        {
            EmployeeForename = "John",
            EmployeSurname = "Doe",
            PermissionTypeId = 1,
            PermissionDate = DateTime.Today
        };
        var permission = new Permission
        {
            Id = 1,
            EmployeeForename = command.EmployeeForename,
            EmployeSurname = command.EmployeSurname,
            PermissionTypeId = command.PermissionTypeId,
            PermissionDate = command.PermissionDate
        };

        _mockPermissionRepository.Setup(x => x.AddAsync(It.IsAny<Permission>())).Callback<Permission>(p => permission = p);




        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        // Assert
        _mockPermissionRepository.Verify(x => x.AddAsync(It.IsAny<Permission>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Once);
        Assert.AreEqual(permission, result);
    }
}
