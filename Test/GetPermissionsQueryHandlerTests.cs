using Application.Permissions.Querys;
using AutoMapper;
using Domain.Interface.ElasticsearchServices;
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
public class GetPermissionsQueryHandlerTests
{
    private Mock<IElasticSearchApplication<PermissionElasticsearch>> _elasticSearchApplicationMock;
    private Mock<IMapper> _mapperMock;
    private IGetPermissionsQueryHandler _handler;
    private Mock<ILogger<GetPermissionsQueryHandler>> _logger;

    [SetUp]
    public void Setup()
    {
        _elasticSearchApplicationMock = new Mock<IElasticSearchApplication<PermissionElasticsearch>>();
        _mapperMock = new Mock<IMapper>();
        _logger = new Mock<ILogger<GetPermissionsQueryHandler>>();
        _handler = new GetPermissionsQueryHandler(_elasticSearchApplicationMock.Object, _mapperMock.Object, _logger.Object);
    }

    [Test]
    public async Task Handle_ReturnsMappedPermissionResponseList()
    {

        var date = DateTime.UtcNow;
        // Arrange
        var permissions = new List<PermissionElasticsearch>
            {
                new PermissionElasticsearch
                {
                    EmployeeForename = "Carlos",
                    EmployeSurname = "Castilla",
                    PermissionType = "Permission",
                    PermissionDate = date,
                    Id = 1
                }
            };
        var PermissionResponseList = new List<PermissionResponse>
            {
                new PermissionResponse
                {
                    EmployeeForename = "Carlos",
                    EmployeSurname = "Castilla",
                    PermissionType = "Type2",
                    PermissionDate = date,
                    Id = 1
                }
            };

        _elasticSearchApplicationMock.Setup(x => x.GetAll()).ReturnsAsync(permissions);
        _mapperMock.Setup(x => x.Map<List<PermissionResponse>>(permissions)).Returns(PermissionResponseList);

        // Act
        var result = await _handler.Handle(new GetPermissionsQuery(), CancellationToken.None);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.ElementAt(0).EmployeeForename + " " + result.ElementAt(0).EmployeSurname, Is.EqualTo("Carlos Castilla"));
        Assert.That(result.ElementAt(0).PermissionType, Is.EqualTo("Type2"));
        Assert.That(result.ElementAt(0).PermissionDate, Is.EqualTo(permissions.FirstOrDefault().PermissionDate));
    }
}