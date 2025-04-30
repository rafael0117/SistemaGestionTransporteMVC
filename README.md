# Sistema de Gestion y Transporte MVC

  <img width="30%" align="right" alt="Github" src="https://user-images.githubusercontent.com/48678280/88862734-4903af80-d201-11ea-968b-9c939d88a37c.gif" />

- ### Lenguaje y Tool
  <img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/csharp/csharp-original.svg" alt="C#" width="40" height="40"/>
  <img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/visualstudio/visualstudio-original.svg" alt="C#" width="40" height="40"/>
  <img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/git/git-original.svg" alt="C#" width="40" height="40"/>
  
- ### Database **SQL**
  <img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/microsoftsqlserver/microsoftsqlserver-original.svg" alt="C#" width="40" height="40"/>
------------------------------------------------------------------------------------
-- Crear la base de datos
CREATE DATABASE bdtransporte;
GO

USE bdtransporte;
GO

-- Tabla tb_destino
CREATE TABLE destino (
    id_destino INT IDENTITY(1,1) PRIMARY KEY,
    nombre_des VARCHAR(100) NOT NULL,
    imagen VARCHAR(255) NULL
);
GO

-- Tabla rol
CREATE TABLE rol (
    idrol INT IDENTITY(1,1) PRIMARY KEY,
    descripcion VARCHAR(255) NULL
);
GO

-- Tabla usuario
CREATE TABLE usuario (
    idusuario BIGINT IDENTITY(1,1) PRIMARY KEY,
    nombres VARCHAR(255) NULL,
    apellidos VARCHAR(255) NULL,
    usuario VARCHAR(255) UNIQUE,
    clave VARCHAR(255) NULL,
    idrol INT NOT NULL,
    correo VARCHAR(255) NOT NULL,
    direccion VARCHAR(255) NULL,
    FOREIGN KEY (idrol) REFERENCES rol(idrol)
);
GO

-- Tabla personal
CREATE TABLE personal (
    id_personal INT IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    apellido VARCHAR(100) NOT NULL,
    dni VARCHAR(20) UNIQUE NOT NULL,
    telefono VARCHAR(20) NULL,
    email VARCHAR(100) NULL,
    direccion VARCHAR(255) NULL,
    idrol INT NOT NULL,
    FOREIGN KEY (idrol) REFERENCES rol(idrol)
);
GO

-- Tabla tb_bus
CREATE TABLE bus (
    id_bus INT IDENTITY(1,1) PRIMARY KEY,
    modelo VARCHAR(50) NOT NULL,
    marca VARCHAR(50) NOT NULL,
    anio INT NOT NULL,
    capacidad INT NOT NULL,
    placa VARCHAR(20) UNIQUE NOT NULL
);
GO

-- Tabla tb_destino ya creada

-- Tabla tb_viaje
CREATE TABLE viaje (
    id_viaje INT IDENTITY(1,1) PRIMARY KEY,
    id_bus INT NOT NULL,
    id_destino INT NOT NULL,
    fech_sal DATE NOT NULL,
    fech_lle DATE NOT NULL,
    incidencias VARCHAR(40) NULL,
    precio FLOAT NOT NULL,
    FOREIGN KEY (id_bus) REFERENCES bus(id_bus),
    FOREIGN KEY (id_destino) REFERENCES destino(id_destino)
);
GO

-- Tabla venta_pasaje
CREATE TABLE venta_pasaje (
    id_venta INT IDENTITY(1,1) PRIMARY KEY,
    estado VARCHAR(50) DEFAULT 'pendiente',
    fecha_venta DATETIME2 NOT NULL,
    total FLOAT NOT NULL,
    id_usuario BIGINT NOT NULL,
    numero VARCHAR(255) NOT NULL,
    FOREIGN KEY (id_usuario) REFERENCES usuario(idusuario)
);
GO

-- Tabla detalle_venta_pasaje
CREATE TABLE detalle_venta_pasaje (
    id_detalle INT IDENTITY(1,1) PRIMARY KEY,
    cantidad INT NOT NULL,
    precio FLOAT NOT NULL,
    total FLOAT NOT NULL,
    id_venta INT NOT NULL,
    id_viaje INT NOT NULL,
    FOREIGN KEY (id_venta) REFERENCES venta_pasaje(id_venta),
    FOREIGN KEY (id_viaje) REFERENCES viaje(id_viaje)
);
GO

-- Tabla tb_cronograma
CREATE TABLE cronograma (
    id_cronograma INT IDENTITY(1,1) PRIMARY KEY,
    fecha DATE NOT NULL,
    hora_llegada TIME NOT NULL,
    hora_salida TIME NOT NULL,
    id_bus INT NOT NULL,
    id_viaje INT NOT NULL,
    FOREIGN KEY (id_bus) REFERENCES bus(id_bus),
    FOREIGN KEY (id_viaje) REFERENCES viaje(id_viaje)
);
GO

-- Tabla tb_revision
CREATE TABLE revision (
    revision_id INT IDENTITY(1,1) PRIMARY KEY,
    fecha_revision DATE NOT NULL,
    observaciones VARCHAR(50) NULL,
    resultado VARCHAR(50) NOT NULL,
    tipo_revision VARCHAR(50) NOT NULL,
    id_bus INT NOT NULL,
    FOREIGN KEY (id_bus) REFERENCES bus(id_bus)
);
GO

-- Tabla viaje (diferente a tb_viaje)
CREATE TABLE viajeD (
    id_viaje INT IDENTITY(1,1) PRIMARY KEY,
    destino VARCHAR(100) NOT NULL,
    estado VARCHAR(50) NOT NULL,
    fecha_llegada DATETIME2 NULL,
    fecha_salida DATETIME2 NOT NULL,
    origen VARCHAR(100) NOT NULL,
    id_bus INT NOT NULL,
    FOREIGN KEY (id_bus) REFERENCES bus(id_bus)
);
GO

-- Trigger para SQL Server (equivalente al de MySQL)
CREATE TRIGGER after_insert_viaje
ON viaje
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @hora_salida TIME;
    DECLARE @hora_llegada TIME;
    DECLARE @rand1 FLOAT;
    DECLARE @rand2 FLOAT;
    DECLARE @segundos_salida INT;
    DECLARE @segundos_llegada INT;
    DECLARE @id_viaje INT;
    DECLARE @fech_sal DATE;
    DECLARE @id_bus INT;

    SELECT TOP 1 
        @id_viaje = inserted.id_viaje,
        @fech_sal = inserted.fech_sal,
        @id_bus = inserted.id_bus
    FROM inserted;

    -- Generar aleatorios
    SET @rand1 = RAND() * 43200; -- hasta 12 horas en segundos
    SET @rand2 = RAND() * 43200;

    SET @segundos_salida = CAST(@rand1 AS INT);
    SET @segundos_llegada = @segundos_salida + CAST(@rand2 AS INT);

    -- Calcular hora de salida y llegada
    SET @hora_salida = DATEADD(SECOND, @segundos_salida, CAST('00:00:00' AS TIME));
    SET @hora_llegada = DATEADD(SECOND, @segundos_llegada, CAST('00:00:00' AS TIME));

    -- Insertar en tb_cronograma
    INSERT INTO cronograma (id_viaje, fecha, hora_salida, hora_llegada, id_bus)
    VALUES (@id_viaje, @fech_sal, @hora_salida, @hora_llegada, @id_bus);
END;
GO

<p>select*from revision</p>

-- Insertar datos en la tabla rol (solo Admin y Cliente)
INSERT INTO rol (descripcion) VALUES 
('Administrador'), 
('Cliente');
GO

-- Insertar datos en la tabla usuario
INSERT INTO usuario (nombres, apellidos, usuario, clave, idrol, correo, direccion) VALUES 
('Juan', 'Pérez', 'juanp', '123456', 1, 'juanp@example.com', 'Av. Siempre Viva 123'),
('Ana', 'Gómez', 'anag', 'abcdef', 2, 'anag@example.com', 'Jr. Las Flores 456');
GO

-- Insertar datos en la tabla personal
-- Insertar datos en la tabla personal (corregido)
INSERT INTO personal (nombre, apellido, dni, telefono, email, direccion, idrol) VALUES
('Carlos', 'Ramírez', '12345678', '987654321', 'carlosr@example.com', 'Calle Los Olivos 789', 1),
('Lucía', 'Fernández', '87654321', '912345678', 'luciaf@example.com', 'Av. Los Cipreses 321', 1);

-- Insertar datos en la tabla destino
INSERT INTO destino (nombre_des, imagen) VALUES 
('Lima', 'lima.jpg'),
('Cusco', 'cusco.jpg'),
('Arequipa', 'arequipa.jpg');
GO

-- Insertar datos en la tabla bus
INSERT INTO bus (modelo, marca, anio, capacidad, placa) VALUES 
('Volvo 9700', 'Volvo', 2020, 50, 'ABC-123'),
('Marcopolo G7', 'Marcopolo', 2019, 48, 'XYZ-789');
GO

-- Insertar datos en la tabla viaje
INSERT INTO viaje (id_bus, id_destino, fech_sal, fech_lle, incidencias, precio) VALUES
(1, 2, '2025-05-10', '2025-05-11', NULL, 150.00),
(2, 3, '2025-05-12', '2025-05-13', 'Retraso por tráfico', 180.00);
GO

-- Insertar datos en la tabla venta_pasaje
INSERT INTO venta_pasaje (estado, fecha_venta, total, id_usuario, numero) VALUES 
('pendiente', '2025-05-01 10:00:00', 150.00, 2, 'VENTA-001'),
('pendiente', '2025-05-02 11:00:00', 180.00, 2, 'VENTA-002');
GO

-- Insertar datos en la tabla detalle_venta_pasaje
INSERT INTO detalle_venta_pasaje (cantidad, precio, total, id_venta, id_viaje) VALUES 
(1, 150.00, 150.00, 1, 1),
(1, 180.00, 180.00, 2, 2);
GO

-- Insertar datos en la tabla revision
INSERT INTO revision (fecha_revision, observaciones, resultado, tipo_revision, id_bus) VALUES
('2025-04-25', 'Sin observaciones', 'Aprobado', 'Técnica', 1),
('2025-04-26', 'Cambio de frenos', 'Aprobado', 'Mecánica', 2);
GO

-- Insertar datos en la tabla viajeD
INSERT INTO viajeD (destino, estado, fecha_llegada, fecha_salida, origen, id_bus) VALUES
('Cusco', 'Programado', '2025-05-11 08:00:00', '2025-05-10 20:00:00', 'Lima', 1),
('Arequipa', 'Programado', '2025-05-13 07:00:00', '2025-05-12 19:00:00', 'Lima', 2);
GO
------------------------------------------------------------------------------------
