CREATE TABLE Manufacturers
(
ManufacturerId INT IDENTITY (1,1) PRIMARY KEY ,
Name NVARCHAR(20),
EstablishedOn DATE
)

INSERT INTO Manufacturers(Name, EstablishedOn)
VALUES
('BMW', '07-03-1916'),
('Tesla', '01-01-2003'),
('Lada', '01-05-1966')

CREATE TABLE Models
(
ModelId INT IDENTITY(100,1)PRIMARY KEY,
Name NVARCHAR(20),
ManufacturerID INT CONSTRAINT FK_Models_Manufacturers FOREIGN KEY REFERENCES Manufacturers(ManufacturerId)
)

INSERT INTO Models( Name, ManufacturerID)
VALUES
('X1',1),
('i6',1),
('Model S',2),
('Model X',2),
('Models 3',2),
('Nova',3)