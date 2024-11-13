# CFA Gestión de Clientes

# Este proyecto es una aplicación web de gestión de clientes desarrollada con ASP.NET Core y Blazor Server. La aplicación permite realizar operaciones CRUD (Crear, Leer, Actualizar, Eliminar) sobre los registros de clientes, utilizando Entity Framework Core y MySQL como base de datos.

<!-- Características

Interfaz de usuario construida con Blazor Server para una experiencia de usuario interactiva y moderna.

Entity Framework Core como ORM para manejar las operaciones de base de datos.

Validaciones de entrada con DataAnnotations.

Páginas para registrar clientes, listar clientes y gestionar información detallada.

Tecnologías utilizadas

ASP.NET Core 7.0

Blazor Server

Entity Framework Core

MySQL

Bootstrap (para el diseño de la interfaz)

Instalación y configuración

Prerrequisitos

.NET SDK 7.0 o superior.

MySQL Server.

Un editor de código, como Visual Studio o Visual Studio Code.

Pasos de instalación

Clona el repositorio:

git clone https://github.com/tuusuario/CFAGestionClientes.git
cd CFAGestionClientes

Configura la cadena de conexión:

Modifica el archivo appsettings.json para agregar tu cadena de conexión a MySQL:

"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=CFA_Clientes;User=root;Password=tu_password;"
}

-- Crear la base de datos y usarla
CREATE DATABASE CFA_Clientes;
USE CFA_Clientes;

-- Crear la tabla Clientes
CREATE TABLE Clientes (
  Codigo INT AUTO_INCREMENT PRIMARY KEY,
  TipoDocumento ENUM('CC', 'TI', 'RC') NOT NULL,
  NumeroDocumento BIGINT NOT NULL UNIQUE,
  Nombres VARCHAR(30) NOT NULL,
  Apellido1 VARCHAR(30) NOT NULL,
  Apellido2 VARCHAR(30),
  Genero ENUM('F', 'M') NOT NULL,
  Email VARCHAR(50) UNIQUE,
  FechaNacimiento DATE NOT NULL,
  UNIQUE (Codigo)
);

-- Crear la tabla Direcciones con referencia a Clientes
CREATE TABLE Direcciones (
  Id INT AUTO_INCREMENT PRIMARY KEY,
  CodigoCliente INT,
  Direccion VARCHAR(100) NOT NULL,
  FOREIGN KEY (CodigoCliente) REFERENCES Clientes(Codigo)
);

-- Crear la tabla Telefonos con referencia a Clientes
CREATE TABLE Telefonos (
  Id INT AUTO_INCREMENT PRIMARY KEY,
  CodigoCliente INT,
  Telefono BIGINT NOT NULL,
  FOREIGN KEY (CodigoCliente) REFERENCES Clientes(Codigo)
);


Restaura las dependencias:

dotnet restore

Aplica las migraciones y actualiza la base de datos:

dotnet ef database update

Ejecuta la aplicación:

dotnet run

Accede a la aplicación:

Abre tu navegador y ve a http://localhost:5000 o el puerto asignado.

Estructura del proyecto

CFAGestionClientes/
├── Controllers/
├── Data/
│   └── CFAContext.cs
├── Models/
│   ├── Cliente.cs
│   ├── Direccion.cs
│   └── Telefono.cs
├── Pages/
│   ├── Clientes.razor
│   └── RegistrarCliente.razor
├── wwwroot/
│   ├── css/
│   └── js/
├── appsettings.json
├── Program.cs
└── README.md

Uso

Registrar Cliente: Navega a http://localhost:5000/registrar-cliente para registrar un nuevo cliente.

Ver Clientes: Navega a http://localhost:5000/clientes para ver la lista de clientes registrados.

Contribuciones

Las contribuciones son bienvenidas. Para contribuir, sigue estos pasos:

Haz un fork del repositorio.

Crea una rama con una nueva funcionalidad: git checkout -b nueva-funcionalidad.

Realiza los cambios y haz commit: git commit -m 'Agrega nueva funcionalidad'.

Haz push a la rama: git push origin nueva-funcionalidad.

Crea un pull request.

Licencia

Este proyecto está licenciado bajo la MIT License.

Contacto

Desarrollado por Ever Quinto -->
