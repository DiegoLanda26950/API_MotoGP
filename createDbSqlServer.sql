CREATE DATABASE MotoGPDB;
USE MotoGPDB;

-- Tabla Equipo
CREATE TABLE Equipo (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100) NOT NULL,
    Pais NVARCHAR(100) NOT NULL,
    Presupuesto FLOAT NOT NULL,
    Victorias INT NOT NULL,
    FechaFundacion DATETIME NOT NULL,
    EsFabricante BIT NOT NULL
);

-- Tabla Moto
CREATE TABLE Moto (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Marca NVARCHAR(100) NOT NULL,
    Modelo NVARCHAR(100) NOT NULL,
    Cilindrada INT NOT NULL,
    Potencia DECIMAL(6,2) NOT NULL,
    Peso FLOAT NOT NULL,
    Anio INT NOT NULL,
    Color NVARCHAR(100) NOT NULL
);

-- Tabla Circuito
CREATE TABLE Circuito (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100) NOT NULL,
    Pais NVARCHAR(100) NOT NULL,
    Ciudad NVARCHAR(100) NOT NULL,
    Longitud FLOAT NOT NULL,
    Curvas INT NOT NULL,
    Homologado BIT NOT NULL,
    FechaInauguracion DATETIME NOT NULL
);

-- Tabla Piloto (con relaciones a las demás tablas)
CREATE TABLE Piloto (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100) NOT NULL,
    Nacionalidad NVARCHAR(100) NOT NULL,
    Dorsal INT NOT NULL,
    CampeonatosGanados INT NOT NULL,
    FechaNacimiento DATETIME NOT NULL,
    Activo BIT NOT NULL,
    MotoId INT NOT NULL,
    EquipoId INT NOT NULL,
    CircuitoId INT NOT NULL,
    FOREIGN KEY (MotoId) REFERENCES Moto(Id),
    FOREIGN KEY (EquipoId) REFERENCES Equipo(Id),
    FOREIGN KEY (CircuitoId) REFERENCES Circuito(Id)
);

-- Datos de ejemplo para Equipo
INSERT INTO Equipo (Nombre, Pais, Presupuesto, Victorias, FechaFundacion, EsFabricante) VALUES
('Repsol Honda', 'Japón', 120000000.0, 300, '1982-01-01', 1),
('Ducati Lenovo', 'Italia', 95000000.0, 180, '2003-01-01', 1),
('Monster Yamaha', 'Japón', 85000000.0, 210, '1999-01-01', 1);

-- Datos de ejemplo para Moto
INSERT INTO Moto (Marca, Modelo, Cilindrada, Potencia, Peso, Anio, Color) VALUES
('Honda', 'RC213V', 1000, 260.50, 157.0, 2024, 'Rojo y Blanco'),
('Ducati', 'Desmosedici GP24', 1000, 270.80, 157.0, 2024, 'Rojo'),
('Yamaha', 'YZR-M1', 1000, 255.30, 157.0, 2024, 'Azul y Negro');

-- Datos de ejemplo para Circuito
INSERT INTO Circuito (Nombre, Pais, Ciudad, Longitud, Curvas, Homologado, FechaInauguracion) VALUES
('Circuit de Barcelona-Catalunya', 'España', 'Montmeló', 4.655, 16, 1, '1991-09-10'),
('Autodromo del Mugello', 'Italia', 'Scarperia', 5.245, 15, 1, '1974-03-01'),
('Circuito de Jerez', 'España', 'Jerez de la Frontera', 4.423, 13, 1, '1986-10-04');

-- Datos de ejemplo para Piloto
INSERT INTO Piloto (Nombre, Nacionalidad, Dorsal, CampeonatosGanados, FechaNacimiento, Activo, MotoId, EquipoId, CircuitoId) VALUES
('Marc Márquez', 'Española', 93, 8, '1993-02-17', 1, 1, 1, 1),
('Francesco Bagnaia', 'Italiana', 1, 2, '1997-01-14', 1, 2, 2, 2),
('Fabio Quartararo', 'Francesa', 20, 1, '1999-04-20', 1, 3, 3, 3);

-- DROP en orden correcto (respetar foreign keys)
-- DROP TABLE IF EXISTS Piloto;
-- DROP TABLE IF EXISTS Equipo;
-- DROP TABLE IF EXISTS Moto;
-- DROP TABLE IF EXISTS Circuito;