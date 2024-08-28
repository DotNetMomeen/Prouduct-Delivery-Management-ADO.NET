
CREATE DATABASE ProuductDelivery
GO

USE ProuductDelivery
GO

CREATE TABLE companyInfo
(
	companyId int NOT NULL PRIMARY KEY,
	companyName varchar (50) NOT NULL
)
go

insert into companyInfo Values (1,'Aarong')
Insert into companyInfo Values (2,'Freeland')
Insert into companyInfo Values (3,'Gentle Park')

GO
CREATE TABLE [Location]
(
	LocationId int PRIMARY KEY NOT NULL,
	LocationName nvarchar(50) not null
)
GO


Insert into [Location] Values (3,'Banani')
Insert into [Location] Values (4,'Agargaon')
Insert into [Location] Values (5,'Uttara')
Insert into [Location] Values (1,'Dhanmondi')
Insert into [Location] Values (2,'Gulshan')

CREATE TABLE Product
(
	ProductId int PRIMARY KEY NOT NULL,
	ProductName varchar(50) NOT NULL,
	companyId int REFERENCES companyInfo(companyId),
	ProductImage varbinary (max) ,
	price money NOT NULL
)
GO

CREATE TABLE ProductDelivery
(
	id int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	ProductId int references Product(ProductId),
	DeliverySchedule nvarchar(50) NOT NULL,
	LocationId int references [Location](LocationId)	
)
GO
