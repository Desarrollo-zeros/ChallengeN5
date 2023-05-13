using Api;
using Application.Permissions.Querys;
using Domain.Permissions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using System.Net.Http.Headers;
using System.Net;
using System.Net.Http.Json;
using Application.Permissions.Commands;

namespace IntegrationTests
{
    public class PermissionsControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly Mock<IMediator> _mediatorMock;
        public PermissionsControllerTests(WebApplicationFactory<Startup> factory)
        {
            
            _factory = factory;
            _mediatorMock = new Mock<IMediator>();
        }

        [Fact]
        public async Task GetPermissions_ReturnsSuccessResult()
        {
            // Crea una instancia del cliente HTTP a través del factory
            var client = _factory.CreateClient();

            // Realiza la petición HTTP GET a la acción GetPermissions del controlador
            var response = await client.GetAsync("/api/permissions");

            // Verifica que la respuesta HTTP sea 200 OK
            response.EnsureSuccessStatusCode();

            // Verifica que el contenido de la respuesta HTTP pueda ser deserializado a una lista de Permission
            var permissions = await response.Content.ReadFromJsonAsync<List<PermissionResponse>>();

            // Verifica que la lista de Permission no sea nula
            Assert.NotNull(permissions);
        }

        [Fact]
        public async Task GetPermissions_ReturnsExpectedContentType()
        {
            // Crea una instancia del cliente HTTP a través del factory
            var client = _factory.CreateClient();

            // Realiza la petición HTTP GET a la acción GetPermissions del controlador
            var response = await client.GetAsync("/api/permissions");

            // Verifica que la respuesta HTTP tenga el Content-Type esperado
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task GetPermissions_WithInvalidUri_ReturnsUnauthorized()
        {
            // Crea una instancia del cliente HTTP a través del factory
            var client = _factory.CreateClient();

            // Realiza la petición HTTP GET a la acción GetPermissions del controlador
            var response = await client.GetAsync("/api/permissions/1");

            // Verifica que la respuesta HTTP sea 401 Unauthorized
            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }

        [Fact]
        public async Task AddPermission_ReturnsSuccessResult()
        {
            // Crea una instancia del cliente HTTP a través del factory
            var client = _factory.CreateClient();

            // Crea una nueva instancia de Permission para agregar
            var newPermission = new Permission()
            {
                EmployeeForename = "John"+new Random().Next(1,1000),
                EmployeSurname = "Doe" + new Random().Next(1, 1000),
                PermissionDate = DateTime.Today,
                PermissionTypeId = 1,
            };

            // Realiza la petición HTTP POST a la acción AddPermission del controlador
            var response = await client.PostAsJsonAsync("/api/permissions", newPermission);

            // Verifica que la respuesta HTTP sea 201 Created
            response.EnsureSuccessStatusCode();

            // Verifica que el contenido de la respuesta HTTP pueda ser deserializado a una instancia de PermissionResponse
            var permissionResponse = await response.Content.ReadFromJsonAsync<PermissionResponse>();

            // Verifica que la instancia de PermissionResponse no sea nula
            Assert.NotNull(permissionResponse);

            // Verifica que el id de la instancia de PermissionResponse sea mayor a cero
            Assert.True(permissionResponse.Id > 0);
        }

        [Fact]
        public async Task ModifyPermission_ReturnsSuccessResult()
        {
            // Crea una instancia del cliente HTTP a través del factory
            var client = _factory.CreateClient();

            // Configura la cabecera "Authorization" con un token válido si es necesario
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "token");

            // Crea un objeto Permission para modificar
            var newPermission = new Permission()
            {
                EmployeeForename = "John" + new Random().Next(1, 1000),
                EmployeSurname = "Doe" + new Random().Next(1, 1000),
                PermissionDate = DateTime.Today,
                PermissionTypeId = 1,
                Id = 1,
                
            };


            // Realiza la petición HTTP PUT a la acción ModifyPermission del controlador
            var response = await client.PutAsJsonAsync($"/api/permissions/{newPermission.Id}", newPermission);

            // Verifica que la respuesta HTTP sea 200 OK
            response.EnsureSuccessStatusCode();

            // Verifica que el contenido de la respuesta HTTP pueda ser deserializado a un objeto Permission
            var modifiedPermission = await response.Content.ReadFromJsonAsync<PermissionResponse>();

            // Verifica que el objeto Permission tenga los datos actualizados
            Assert.Equal(newPermission.Id, modifiedPermission.Id);
            Assert.Equal(newPermission.EmployeSurname, modifiedPermission.EmployeSurname);
            //Assert.Equal(newPermission.PermissionType, modifiedPermission.PermissionType);
        }

        [Fact]
        public async Task RequestPermission_WithInvalidData_ReturnsBadRequest()
        {
            // Crea una instancia del cliente HTTP a través del factory
            var client = _factory.CreateClient();

            // Crea una solicitud HTTP POST con datos inválidos
            var command = new RequestPermissionCommand { PermissionTypeId = 99 };
            var response = await client.PostAsJsonAsync("/api/permissions", command);

            // Verifica que la respuesta HTTP sea 400 Bad Request
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ModifyPermission_WithInvalidId_ReturnsBadRequest()
        {
            // Crea una instancia del cliente HTTP a través del factory
            var client = _factory.CreateClient();

            // Crea una solicitud HTTP PUT con un ID inválido
            var command = new ModifyPermissionCommand { Id = -1 };
            var response = await client.PutAsJsonAsync("/api/permissions/0", command);

            // Verifica que la respuesta HTTP sea 400 Bad Request
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task RequestPermission_WithInvalidData_ReturnsBadRequest2()
        {
            // Arrange
            var client = _factory.CreateClient();
            var command = new RequestPermissionCommand
            {
                PermissionTypeId = 1,
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/permissions", command);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ModifyPermission_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            var command = new ModifyPermissionCommand
            {
                Id = 0,
            };

            // Act
            var response = await client.PutAsJsonAsync($"/api/permissions/{command.Id}", command);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

    }
}