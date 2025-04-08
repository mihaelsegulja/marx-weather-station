CREATE DATABASE dbMarxWeatherStation
GO

USE dbMarxWeatherStation
GO

CREATE TABLE [WeatherData] (
	[Id] int IDENTITY(1,1) PRIMARY KEY,
	[Timestamp] datetime2 DEFAULT GETUTCDATE(),
    [IAQ] float NOT NULL, 
    [StaticIAQ] float NOT NULL,
    [CO2Equivalent] float NOT NULL,
    [BreathVOCEquivalent] float NOT NULL,
    [CompensatedTemperature] float NOT NULL,
    [Pressure] float NOT NULL,
    [CompensatedHumidity] float NOT NULL,
    [GasResistance] float NOT NULL
)
GO