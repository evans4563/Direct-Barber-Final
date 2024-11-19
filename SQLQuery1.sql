CREATE DATABASE DirectBarber1;
use DirectBarber1;

--DROPS ----------------------------------------------------------------------------------------------------------------------
Drop table Solicitud;
Drop table TipoSer;
Drop table EstadoSolicitud;
Drop table Resenas;
Drop table Usuario;
Drop table Rol;

TRUNCATE TABLE TipoSer;
DELETE FROM TipoSer;

--SELECTS ----------------------------------------------------------------------------------------------------------------------
SELECT * FROM Rol;
SELECT * FROM Usuario;
SELECT * FROM Resenas;
SELECT * FROM Solicitud;
SELECT * FROM TipoSer;
SELECT * FROM EstadoSolicitud;



--CREATES = ----------------------------------------------------------------------------------------------------------------------
CREATE TABLE Rol (
    Id INT PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL
);


CREATE TABLE Usuario (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(50),
    apellido NVARCHAR(50),
    correo NVARCHAR(60) NOT NULL,
    contrasena NVARCHAR(200) NOT NULL,
    direccion NVARCHAR(50),
    telefono NVARCHAR(20),
    fec_registro DATETIME DEFAULT GETDATE(),
    fec_nacimiento DATE,
    calificacion DECIMAL(3,2),
    foto VARBINARY(MAX),
    documento NVARCHAR(10),
	Descripcion NVARCHAR(MAX),
    Id_Rol INT NOT NULL,
    FOREIGN KEY (Id_Rol) REFERENCES Rol(Id)
);

CREATE TABLE Resenas (
Id INT IDENTITY(1,1) PRIMARY KEY,
Contenido NVARCHAR(max) NOT NULL,
Calificacion INT NOT NULL,
FechaPublicacion DateTime,
Id_Cliente INT NOT NULL,
Id_Barbero INT NOT NULL
    FOREIGN KEY (id_Cliente) REFERENCES Usuario(Id),
    FOREIGN KEY (id_Barbero) REFERENCES Usuario(Id),
);



CREATE TABLE TipoSer (
    id INT PRIMARY KEY IDENTITY(1,1), -- Identificador único, autoincremental
    nombre NVARCHAR(100) NOT NULL,    -- Nombre del tipo de servicio
    foto NVARCHAR(MAX),               -- Ruta o URL de la foto
    precio DECIMAL(10, 2) NOT NULL    -- Precio del servicio con dos decimales
);

CREATE TABLE EstadoSolicitud (
id INT PRIMARY KEY IDENTITY (1,1),
estado NVARCHAR(20)
);

CREATE TABLE Solicitud (
    id_Solicitud INT IDENTITY(1,1) PRIMARY KEY,
	id_Estado INT DEFAULT 1,
	id_Cliente INT NULL,
    id_Barbero INT NULL,
	precio_Servicio DECIMAL,
	tipo_Servicio INT,
	Dirección NVarchar(MAX) NOT NULL,
    fecha DATETIME,
    descripcion NVARCHAR(200),
    FOREIGN KEY (id_Cliente) REFERENCES Usuario(Id),
    FOREIGN KEY (id_Barbero) REFERENCES Usuario(Id),
	FOREIGN KEY (tipo_servicio) REFERENCES TipoSer(id),
	FOREIGN KEY (id_Estado) REFERENCES EstadoSolicitud(id)
);



-- INSERTS ----------------------------------------------------------------------------------------------------------------------
INSERT INTO Rol (Id,Nombre)
VALUES (1,'Cliente'), (2,'Barbero');


INSERT INTO estadoSolicitud (estado)
VALUES ('Pendiente'),('Completado');