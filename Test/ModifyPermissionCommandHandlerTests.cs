using Application.Permissions.Commands;
using Application.Permissions.Querys;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interface.ElasticsearchServices;
using Domain.Interface.Permissions;
using Domain.Interface.UnitOfWorks;
using Domain.Mappers;
using Domain.Permissions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Security;

[TestFixture]
public class ModifyPermissionCommandHandlerTests
{
    private Mock<IPermissionRepository> _mockPermissionRepository;
    private Mock<IPermissionTypeRepository> _mockPermissionTypeRepository;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private IModifyPermissionCommandHandler _handler;
    private Mock<IElasticSearchApplication<PermissionElasticsearch>> _elasticSearchApplication;
    private IMapper _mapper;
    private Mock<ILogger<ModifyPermissionCommandHandler>> _logger;

    [SetUp]
    public void Setup()
    {
        _mockPermissionRepository = new Mock<IPermissionRepository>();
        _mockPermissionTypeRepository = new Mock<IPermissionTypeRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PermissionMapper>();

        });
        _mapper = config.CreateMapper();

        _elasticSearchApplication = new Mock<IElasticSearchApplication<PermissionElasticsearch>>();
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
        _logger = new Mock<ILogger<ModifyPermissionCommandHandler>>();
        _mockPermissionRepository.Setup(x => x.GetByIdAsync(permission.Id)).ReturnsAsync(permission);
        _mockUnitOfWork.Setup(x => x.PermissionRepository).Returns(_mockPermissionRepository.Object);
        _mockUnitOfWork.Setup(x => x.PermissionTypeRepository.AnyAsync(1)).ReturnsAsync(true);
        _handler = new ModifyPermissionCommandHandler(_mockPermissionRepository.Object,  _elasticSearchApplication.Object, _mockUnitOfWork.Object, _mapper, _logger.Object);


    }


    [Test]
    public async Task Handle_WhenPermissionExists_ShouldUpdatePermissionAndCommitChanges()
    {
        

        var command = new ModifyPermissionCommand()
        {
            PermissionDate = new DateTime(2022, 01, 01),
            PermissionTypeId = 1,
            EmployeSurname = "Doe",
            EmployeeForename = "John",
            Id = 1
        };

     
        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        _mockPermissionRepository.Verify(x => x.GetByIdAsync(1), Times.Once);
        _mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Once);
        Assert.AreEqual(command.EmployeeForename, result.EmployeeForename);
        Assert.AreEqual(command.EmployeSurname, result.EmployeSurname);
        Assert.AreEqual(command.PermissionDate, result.PermissionDate);
    }

    [Test]
    public void Handle_WhenPermissionDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var permissionId = 1;
        _ = _mockPermissionRepository.Setup(x => x.GetByIdAsync(permissionId)).ReturnsAsync(null as Permission);
        var command = new ModifyPermissionCommand()
        {
            PermissionDate = new DateTime(2022, 01, 01),
            PermissionTypeId = 2,
            EmployeSurname = "Doe",
            EmployeeForename = "John",
            Id = permissionId
        };
        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, default));
    }
}