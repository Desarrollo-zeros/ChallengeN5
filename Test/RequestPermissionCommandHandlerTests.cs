using Application.ElasticsearchServices;
using Application.Permissions.Commands;
using AutoMapper;
using Domain.Interface.ElasticsearchServices;
using Domain.Interface.Permissions;
using Domain.Interface.UnitOfWorks;
using Domain.Mappers;
using Domain.Permissions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

[TestFixture]
public class RequestPermissionCommandHandlerTests
{

    private Mock<IPermissionRepository> _mockPermissionRepository;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private IRequestPermissionCommandHandler _handler;
    private Mock<IElasticSearchApplication<PermissionElasticsearch>> _elasticSearchApplication;
    private IMapper _mapper;
    private Mock<ILogger<RequestPermissionCommandHandler>> _logger;
    [SetUp]
    public void Setup()
    {
        _mockPermissionRepository = new Mock<IPermissionRepository>();
        
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
        _mockPermissionRepository.Setup(x => x.GetByIdAsync(permission.Id)).ReturnsAsync(permission);
        _mockUnitOfWork.Setup(x => x.PermissionRepository).Returns(_mockPermissionRepository.Object);
        _mockUnitOfWork.Setup(x => x.PermissionTypeRepository.AnyAsync(1)).ReturnsAsync(true);
        _logger = new Mock<ILogger<RequestPermissionCommandHandler>>();
        _handler = new RequestPermissionCommandHandler(_mockPermissionRepository.Object, _elasticSearchApplication.Object, _mockUnitOfWork.Object, _mapper, _logger.Object);
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
            PermissionDate = DateTime.Today,
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
        _mockPermissionRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Permission, bool>>>()))
                                       .ReturnsAsync(new List<Permission>() {  });

        _mockUnitOfWork.Setup(x => x.CommitAsync()).ReturnsAsync(1);

        _mockPermissionRepository.Setup(x => x.GetByIdAsync(1))
                                      .ReturnsAsync(permission);

        _mockUnitOfWork.Setup(x => x.PermissionTypeRepository.GetByIdAsync(1)).ReturnsAsync(new PermissionType { Id = 1, Description = "Ejemplo"});

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        // Assert
        _mockPermissionRepository.Verify(x => x.AddAsync(It.IsAny<Permission>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Once);
        Assert.AreEqual(permission.EmployeSurname, result.EmployeSurname);
    }
}
