--Create Database CourierMax
--GO

USE CourierMax
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Cities]') AND type in (N'U'))
CREATE TABLE Cities (
    CityId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE
);

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CityDistances]') AND type in (N'U'))
CREATE TABLE CityDistances (
    OriginCityId INT NOT NULL,
    DestinationCityId INT NOT NULL,
    DistanceKm INT NOT NULL,
    DistanceTariff DECIMAL(18,2) NOT NULL,
    CONSTRAINT PK_CityDistances PRIMARY KEY (OriginCityId, DestinationCityId),
    CONSTRAINT FK_CityDistances_Origin FOREIGN KEY (OriginCityId) REFERENCES Cities(CityId),
    CONSTRAINT FK_CityDistances_Destination FOREIGN KEY (DestinationCityId) REFERENCES Cities(CityId)
);

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id= OBJECT_ID(N'[dbo].[Vehicles]') AND type in (N'U'))
CREATE TABLE Vehicles (
    VehicleId INT IDENTITY(1,1) PRIMARY KEY,
    LicensePlate NVARCHAR(7) NOT NULL UNIQUE, 
    DriverName NVARCHAR(150) NOT NULL,
    MaxWeightKg DECIMAL(5,2) NOT NULL,
    MaxVolumeM3 DECIMAL(5,2) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id= OBJECT_ID(N'[dbo].[Shipments]') AND type in (N'U'))
CREATE TABLE Shipments (
    ShipmentId INT IDENTITY(1,1) PRIMARY KEY,
    TrackingCode NVARCHAR(11) NOT NULL UNIQUE,  
    SenderName NVARCHAR(150) NOT NULL,
    SenderPhone NVARCHAR(10) NOT NULL,
    SenderAddress NVARCHAR(250) NOT NULL,
    ReceiverName NVARCHAR(150) NOT NULL,
    ReceiverPhone NVARCHAR(10) NOT NULL,
    ReceiverAddress NVARCHAR(250) NOT NULL,
    OriginCityId INT NOT NULL,
    DestinationCityId INT NOT NULL,
    WeightKg DECIMAL(5,2) NOT NULL,
    LengthCm DECIMAL(5,2) NOT NULL,
    WidthCm DECIMAL(5,2) NOT NULL,
    HeightCm DECIMAL(5,2) NOT NULL,
    PackageType NVARCHAR(20) NOT NULL,
    ServiceType NVARCHAR(20) NOT NULL,
    BaseTariff DECIMAL(18,2) NOT NULL,
    WeightSurcharge DECIMAL(18,2) NOT NULL,
    DistanceSurcharge DECIMAL(18,2) NOT NULL,
    PackageTypeSurcharge DECIMAL(18,2) NOT NULL,
    TotalCost DECIMAL(18,2) NOT NULL,
    CurrentStatus NVARCHAR(20) NOT NULL DEFAULT 'CREADO',
    VehicleId INT NULL,    
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    EstimatedDeliveryDate DATETIME2 NOT NULL,
    DeliveredAt DATETIME2 NULL,
    
    CONSTRAINT FK_Shipments_Origin FOREIGN KEY (OriginCityId) REFERENCES Cities(CityId),
    CONSTRAINT FK_Shipments_Destination FOREIGN KEY (DestinationCityId) REFERENCES Cities(CityId),
    CONSTRAINT FK_Shipments_Vehicle FOREIGN KEY (VehicleId) REFERENCES Vehicles(VehicleId)
);

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id= OBJECT_ID(N'[dbo].[ShipmentStatusLogs]') AND type in (N'U'))
CREATE TABLE ShipmentStatusLogs (
    StatusLogId INT IDENTITY(1,1) PRIMARY KEY,
    ShipmentId INT NOT NULL,
    PreviousStatus NVARCHAR(20) NULL,
    NewStatus NVARCHAR(20) NOT NULL,
    ChangedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    ChangeReason NVARCHAR(500) NULL,
    UserId NVARCHAR(100) NOT NULL,
    CONSTRAINT FK_StatusLogs_Shipments FOREIGN KEY (ShipmentId) REFERENCES Shipments(ShipmentId)
);