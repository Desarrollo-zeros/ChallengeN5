using Application.Permissions.Querys;
using AutoMapper;
using Domain.Interface.ElasticsearchServices;
using Domain.Interface.Permissions;
using Domain.Mappers;
using Domain.Permissions;
using Microsoft.Extensions.Logging;
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
  
    private IGetPermissionsFilterQueryHandler _handler;
    private IMapper _mapper;
    private Mock<IElasticSearchApplication<PermissionElasticsearch>> _elasticsearchSearchApplication;
   
    [SetUp]
    public void Setup()
    {
        

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PermissionMapper>();

        });
        _mapper = config.CreateMapper();
        _elasticsearchSearchApplication = new Mock<IElasticSearchApplication<PermissionElasticsearch>>();
        _handler = new GetPermissionsFilterQueryHandler(_elasticsearchSearchApplication.Object, _mapper);
    }

    [Test]
    public async Task Handle_ValidId_ReturnsPermission()
    {
        // Arrange
        var permission = new PermissionElasticsearch { Id = 1 };
        _elasticsearchSearchApplication.Setup(x => x.GetAsync(1))
                                 .ReturnsAsync(permission);

        var query = new GetPermissionsFilterQuery(1) {  };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result.EmployeSurname, Is.EqualTo(permission.EmployeSurname));
    }

    [Test]
    public async Task Handle_InvalidId_ReturnsNull()
    {
       
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
        var permission = new PermissionElasticsearch
        {
            Id = id,
            EmployeeForename = "John",
            EmployeSurname = "Doe",
            PermissionType = "Permission",
            PermissionDate = DateTime.UtcNow
        };
        _elasticsearchSearchApplication.Setup(x => x.GetAsync(id))
                                 .ReturnsAsync(permission);

        var query = new GetPermissionsFilterQuery(id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Id, Is.EqualTo(permission.Id));
        Assert.That(result.EmployeeForename, Is.EqualTo(permission.EmployeeForename));
        Assert.That(result.EmployeSurname, Is.EqualTo(permission.EmployeSurname));
        Assert.That(result.PermissionType, Is.EqualTo(permission.PermissionType));
        Assert.That(result.PermissionDate, Is.EqualTo(permission.PermissionDate));
    }

    [Test]
    public async Task Handle_InvalidId_ReturnsNull2()
    {
        // Arrange
        int id = new Random().Next(1, 100);
        _elasticsearchSearchApplication.Setup(x => x.GetAsync(id))
                                 .ReturnsAsync((PermissionElasticsearch)null);

        var query = new GetPermissionsFilterQuery(id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsNull(result);
    }
}
