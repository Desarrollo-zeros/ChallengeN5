# Web API de Registro de Permisos de Usuarios

Esta es una Web API desarrollada en .NET Core para registrar permisos de usuarios. A continuación se detallan los pasos para configurar y utilizar la API.

## Requisitos

- Docker
- Docker Compose

## Configuración

1. Clonar el repositorio en tu equipo.
2. Navegar a la carpeta del repositorio.
3. Ejecutar el comando `docker-compose up` para construir y levantar los contenedores de la API y las herramientas adicionales.

## Estructura de la Base de Datos

La API utiliza una base de datos SQL Server y consta de dos tablas: `Permissions` y `PermissionTypes`. La tabla `Permissions` tiene los siguientes campos:

- `id`: Identificador único del permiso (tipo entero).
- `EmployeeForename`: Nombre del empleado (texto, no nulo).
- `EmployeeSurname`: Apellido del empleado (texto, no nulo).
- `PermissionType`: Tipo de permiso (tipo entero, no nulo).
- `PermissionDate`: Fecha en que se concedió el permiso (tipo fecha, no nulo).

La tabla `PermissionTypes` tiene los siguientes campos:

- `id`: Identificador único del tipo de permiso (tipo entero).
- `Description`: Descripción del tipo de permiso (texto, no nulo).

La relación entre `Permissions` y `PermissionTypes` se establece mediante la clave foránea `PermissionType`.

## Servicios

La API cuenta con tres servicios principales:

### Request Permission

Este servicio permite registrar un nuevo permiso en la base de datos. La información del permiso se debe enviar en el cuerpo de la petición en formato JSON. El servicio también persiste un registro del permiso en un índice de Elasticsearch y publica un mensaje en un topic de Kafka con la información de la operación realizada.

### Modify Permission

Este servicio permite modificar un permiso existente en la base de datos. La información del permiso modificado se debe enviar en el cuerpo de la petición en formato JSON, y se debe especificar el identificador único del permiso que se quiere modificar. El servicio también persiste un registro de la modificación del permiso en un índice de Elasticsearch y publica un mensaje en un topic de Kafka con la información de la operación realizada.

### Get Permissions

Este servicio permite obtener una lista de todos los permisos registrados en la base de datos. La respuesta se devuelve en formato JSON.

## Tecnologías Utilizadas

La API utiliza las siguientes tecnologías:

- .NET Core
- SQL Server
- Entity Framework
- Elasticsearch
- Kafka
- Docker

## Testing

La API cuenta con pruebas unitarias e integración para cada uno de sus servicios. Los tests pueden ejecutarse mediante el comando `dotnet test`.


## Instrucciones de inicio del proyecto

1. Clonar el repositorio de GitHub:

   ```bash
   git clone https://github.com/Desarrollo-zeros/ChallengeN5
   cd ChallengeN5
2. Ubicarse en la carpeta raíz del proyecto:
   Una vez que hayas clonado el repositorio y configurado el archivo docker-compose.yml, puedes iniciar el proyecto con el siguiente comando en la raíz del proyecto
   ```bash docker-compose up  ```
   
   Esto iniciará todos los contenedores de Docker necesarios para ejecutar la aplicación. Si es la primera vez que ejecutas este comando, puede tardar un tiempo en descargar todas las imágenes de Docker necesarias.
   Una vez que se hayan iniciado los contenedores, la aplicación debería estar disponible en http://localhost:5000. También puedes acceder a Seq (una herramienta de registro de eventos) en http://localhost:5341 y a Elasticsearch en   
   http://localhost:9200.
  ```bash docker-compose down  ```


Puedes detener los contenedores en cualquier momento presionando Ctrl + C en la terminal donde se está ejecutando docker-compose up. Para detener y eliminar todos los contenedores, redes y volúmenes relacionados con este proyecto, puedes ejecutar:

3. Abrir el navegador web y acceder a la URL http://localhost:5000 para probar la API.
Nota: Asegurarse de tener los puertos 5341, 5001, 9200, 2181 y 9092 disponibles en el host local.


Nota: Si necesitas modificar algún archivo de configuración (como appsettings.json), debes detener y volver a iniciar los contenedores para que los cambios surtan efecto.

Nota: El proyecto cuenta con migraciones automáticas, por lo que solo es necesario ejecutarlo.

 ```yml
version: '3.9'

services:
  seq:
    image: datalust/seq:latest
    environment:
      ACCEPT_EULA: "Y"
    ports:
      - 5341:80
    volumes:
      - data:/usr/share/seq/data

  sqldata:
    image: mcr.microsoft.com/mssql/server:2017-latest 
    environment:
      SA_PASSWORD: "Pass@word"
      ACCEPT_EULA: "Y"
    ports:
      - "5001:1433"
    volumes:
      - data:/var/opt/mssql/data

  elasticsearch:
    image: docker.elastic.co/elasticsearch/elasticsearch:7.15.0
    environment:
      - discovery.type=single-node
    ports:
      - 9200:9200
    ulimits:
      memlock:
        soft: -1
        hard: -1
    volumes:
      - data:/usr/share/elasticsearch/data

  api:
    build:
      context: .
      dockerfile: Api/Dockerfile
    ports:
      - 5000:80
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - SQL_SERVER_CONNECTION=Server=sqldata; Database=PermissionDb; User=sa;Password=Pass@word;TrustServerCertificate=True;
      - Elasticsearch__Uri=http://elasticsearch:9200
      - Kafka__BootstrapServers=kafka:9092
      - SEQURL=http://seq
    depends_on:
      - elasticsearch
      - kafka
      - sqldata

  zookeeper:
    image: 'bitnami/zookeeper:latest'
    ports:
      - '2181:2181'
    environment:
      - ALLOW_ANONYMOUS_LOGIN=yes
      - ZOOKEEPER_CLIENT_PORT=2181

  kafka:
    image: confluentinc/cp-kafka:latest
    ports:
      - "9092:9092"
    environment:
      KAFKA_ADVERTISED_LISTENERS: PLAINTEXT://kafka:9092,PLAINTEXT_HOST://localhost:9093
      KAFKA_LISTENER_SECURITY_PROTOCOL_MAP: PLAINTEXT:PLAINTEXT,PLAINTEXT_HOST:PLAINTEXT
      KAFKA_INTER_BROKER_LISTENER_NAME: PLAINTEXT
      KAFKA_ZOOKEEPER_CONNECT: zookeeper:2181
    depends_on:
       - zookeeper

volumes:
    data:
 ```


Nota:
Una vez que hayas configurado el entorno de desarrollo y hayas iniciado todos los servicios a través del comando "docker-compose up", puedes acceder a la interfaz web de Seq ingresando en tu navegador la dirección "http://localhost:5341". Aquí, podrás ver todos los registros generados por la API en tiempo real y utilizar la búsqueda y el filtrado para encontrar información específica.
En resumen, Seq es una herramienta muy útil para el seguimiento y análisis de registros en aplicaciones modernas, y en este proyecto en particular se utiliza para monitorear y analizar los registros generados por la API.



Este código es un ejemplo de un controlador en la API que maneja las solicitudes HTTP relacionadas con permisos y registros de auditoría. El controlador está etiquetado con la ruta base "api/permissions" y es un controlador de tipo ApiController de ASP.NET Core.

A continuación, se describen los endpoints disponibles en la API:

POST /api/Permissions
Endpoint para solicitar un permiso. Recibe en el cuerpo de la petición un objeto JSON con la información del permiso a solicitar. Retorna el resultado de la solicitud.

PUT /api/Permissions/{id}
Endpoint para modificar un permiso existente. Recibe como parámetro en la URL el Id del permiso a modificar y en el cuerpo de la petición un objeto JSON con la información del permiso actualizado. Retorna el resultado de la modificación.

GET /api/Permissions
Endpoint para obtener todos los permisos registrados. Retorna una lista de objetos JSON con la información de los permisos registrados.

PATCH /api/Permissions
Endpoint para obtener los logs de la aplicación. Retorna una lista de objetos JSON con la información de los logs.


 ```
| Método HTTP | Ruta                          | Descripción                                |
|-------------|-------------------------------|--------------------------------------------|
| POST        | /api/permissions              | Solicitar un nuevo permiso                 |
| PUT         | /api/permissions/{id}         | Modificar un permiso existente             |
| GET         | /api/permissions              | Obtener una lista de permisos existentes   |
| PATCH       | /api/permissions              | Obtener una lista de registros de auditoría|
```

## Contribución

Si quieres contribuir a este proyecto, por favor lee el archivo CONTRIBUTING.md para conocer los pasos a seguir.

## Licencia

Este proyecto está licenciado bajo la Licencia MIT. Para más información, consulta el archivo LICENSE.
