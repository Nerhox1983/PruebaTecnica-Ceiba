USE CourierMax;
GO

INSERT INTO Cities (Name) 
VALUES 
	('Bogotá')
	, ('Medellín')
	, ('Cali')
	, ('Barranquilla');

INSERT INTO CityDistances (OriginCityId, DestinationCityId, DistanceKm, DistanceTariff)
VALUES 
	((SELECT CityId FROM Cities WHERE Name='Bogotá'), (SELECT CityId FROM Cities WHERE Name='Medellín'), 480, 12000)
	, ((SELECT CityId FROM Cities WHERE Name='Bogotá'), (SELECT CityId FROM Cities WHERE Name='Cali'), 360, 9000)
	, ((SELECT CityId FROM Cities WHERE Name='Bogotá'), (SELECT CityId FROM Cities WHERE Name='Barranquilla'), 950, 20000)
	, ((SELECT CityId FROM Cities WHERE Name='Medellín'), (SELECT CityId FROM Cities WHERE Name='Cali'), 310, 8000)
	, ((SELECT CityId FROM Cities WHERE Name='Medellín'), (SELECT CityId FROM Cities WHERE Name='Barranquilla'), 650, 15000)
	, ((SELECT CityId FROM Cities WHERE Name='Cali'), (SELECT CityId FROM Cities WHERE Name='Barranquilla'), 900, 18000);

INSERT INTO Vehicles (LicensePlate, DriverName, MaxWeightKg, MaxVolumeM3, IsActive)
VALUES 
	('ABC-123', 'Juan Pérez', 500.00, 10.00, 1)
	, ('DEF-456', 'María López', 300.00, 6.00, 1)
	, ('GHI-789', 'Carlos Ruiz', 800.00, 15.00, 1);