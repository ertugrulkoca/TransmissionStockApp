BEGIN TRAN;

----------------------------------------------------------------
-- 0) Ön temizlik (sadece test için, TR-00.. numaralıları sil)
----------------------------------------------------------------
-- DELETE FROM TransmissionStockLocations WHERE TransmissionStockId IN (SELECT Id FROM TransmissionStocks WHERE SparePartNo LIKE 'TR-00%');
-- DELETE FROM TransmissionStocks WHERE SparePartNo LIKE 'TR-00%';
-- DELETE FROM VehicleModels WHERE Name LIKE 'Test%';
-- DELETE FROM VehicleBrands WHERE Name IN ('BMW','Mercedes','Toyota','Renault','VW','Skoda','Peugeot','Opel','Nissan','Audi');

----------------------------------------------------------------
-- 1) Lookup tabloları insert et (yoksa)
----------------------------------------------------------------

-- Transmission Brands
IF NOT EXISTS (SELECT 1 FROM TransmissionBrands WHERE Name='ZF')
INSERT INTO TransmissionBrands (Name) VALUES ('ZF');
IF NOT EXISTS (SELECT 1 FROM TransmissionBrands WHERE Name='AISIN')
INSERT INTO TransmissionBrands (Name) VALUES ('AISIN');
IF NOT EXISTS (SELECT 1 FROM TransmissionBrands WHERE Name='JATCO')
INSERT INTO TransmissionBrands (Name) VALUES ('JATCO');
IF NOT EXISTS (SELECT 1 FROM TransmissionBrands WHERE Name='GETRAK')
INSERT INTO TransmissionBrands (Name) VALUES ('GETRAK');

-- Vehicle Brands
IF NOT EXISTS (SELECT 1 FROM VehicleBrands WHERE Name='BMW')
INSERT INTO VehicleBrands (Name) VALUES ('BMW');
IF NOT EXISTS (SELECT 1 FROM VehicleBrands WHERE Name='Mercedes')
INSERT INTO VehicleBrands (Name) VALUES ('Mercedes');
IF NOT EXISTS (SELECT 1 FROM VehicleBrands WHERE Name='Toyota')
INSERT INTO VehicleBrands (Name) VALUES ('Toyota');
IF NOT EXISTS (SELECT 1 FROM VehicleBrands WHERE Name='Renault')
INSERT INTO VehicleBrands (Name) VALUES ('Renault');
IF NOT EXISTS (SELECT 1 FROM VehicleBrands WHERE Name='VW')
INSERT INTO VehicleBrands (Name) VALUES ('VW');
IF NOT EXISTS (SELECT 1 FROM VehicleBrands WHERE Name='Skoda')
INSERT INTO VehicleBrands (Name) VALUES ('Skoda');
IF NOT EXISTS (SELECT 1 FROM VehicleBrands WHERE Name='Peugeot')
INSERT INTO VehicleBrands (Name) VALUES ('Peugeot');
IF NOT EXISTS (SELECT 1 FROM VehicleBrands WHERE Name='Opel')
INSERT INTO VehicleBrands (Name) VALUES ('Opel');
IF NOT EXISTS (SELECT 1 FROM VehicleBrands WHERE Name='Nissan')
INSERT INTO VehicleBrands (Name) VALUES ('Nissan');
IF NOT EXISTS (SELECT 1 FROM VehicleBrands WHERE Name='Audi')
INSERT INTO VehicleBrands (Name) VALUES ('Audi');

----------------------------------------------------------------
-- 2) Id’leri oku
----------------------------------------------------------------
DECLARE @ZFId INT = (SELECT Id FROM TransmissionBrands WHERE Name='ZF');
DECLARE @AisinId INT = (SELECT Id FROM TransmissionBrands WHERE Name='AISIN');
DECLARE @JatcoId INT = (SELECT Id FROM TransmissionBrands WHERE Name='JATCO');
DECLARE @GetrakId INT = (SELECT Id FROM TransmissionBrands WHERE Name='GETRAK');

-- DriveType Id'lerini Value'dan al (2 ve 4 çekiş)
DECLARE @Drive1Id INT = 1;
DECLARE @Drive2Id INT = 2;

----------------------------------------------------------------
-- 3) 40 adet TransmissionStocks ekle
--  VehicleModelId kolonlarını kendi mevcut tablona göre güncellemen lazım.
--  Burada örnek olsun diye VehicleModelId değerleri bırakıldı.
----------------------------------------------------------------
INSERT INTO TransmissionStocks
(SparePartNo, TransmissionBrandId, TransmissionCode, TransmissionNumber, [Year], VehicleBrandId, VehicleModelId, TransmissionStatusId, TransmissionDriveTypeId, [Description]) VALUES
-- Toyota
('TR-0001', @AisinId, 'AISIN-TF60',  'TN-A001', 2018, (SELECT Id FROM VehicleBrands WHERE Name='Toyota'), 219, 2, @Drive2Id, 'Toyota Corolla otomatik'),
('TR-0002', @AisinId, 'AISIN-TF71',  'TN-A002', 2020, (SELECT Id FROM VehicleBrands WHERE Name='Toyota'), 221, 1, @Drive2Id, 'Toyota C-HR otomatik'),
('TR-0003', @AisinId, 'AISIN-TF80',  'TN-A003', 2019, (SELECT Id FROM VehicleBrands WHERE Name='Toyota'), 222, 2, @Drive4Id, 'Toyota RAV4 4x4'),
('TR-0004', @AisinId, 'AISIN-TF70',  'TN-A004', 2017, (SELECT Id FROM VehicleBrands WHERE Name='Toyota'), 224, 3, @Drive2Id, 'Toyota Yaris otomatik'),
-- Renault
('TR-0005', @JatcoId, 'JATCO-CVT7',  'TN-J001', 2017, (SELECT Id FROM VehicleBrands WHERE Name='Renault'), 182, 2, @Drive2Id, 'Renault Clio CVT'),
('TR-0006', @JatcoId, 'JATCO-CVT8',  'TN-J002', 2019, (SELECT Id FROM VehicleBrands WHERE Name='Renault'), 183, 1, @Drive2Id, 'Renault Captur CVT'),
('TR-0007', @JatcoId, 'JATCO-CVT8',  'TN-J003', 2020, (SELECT Id FROM VehicleBrands WHERE Name='Renault'), 185, 2, @Drive2Id, 'Renault Megane Sedan CVT'),
('TR-0008', @JatcoId, 'JATCO-CVT8',  'TN-J004', 2021, (SELECT Id FROM VehicleBrands WHERE Name='Renault'), 186, 1, @Drive2Id, 'Renault Megane E-Tech'),
-- VW
('TR-0009', @GetrakId, 'GETRAK-6DCT250','TN-G001', 2018, (SELECT Id FROM VehicleBrands WHERE Name='VW'), 231, 2, @Drive2Id, 'VW Polo DSG'),
('TR-0010', @GetrakId, 'GETRAK-6DCT450','TN-G002', 2019, (SELECT Id FROM VehicleBrands WHERE Name='VW'), 232, 1, @Drive2Id, 'VW Golf DSG'),
('TR-0011', @GetrakId, 'GETRAK-7DCL750','TN-G003', 2020, (SELECT Id FROM VehicleBrands WHERE Name='VW'), 233, 2, @Drive2Id, 'VW Passat DSG'),
('TR-0012', @GetrakId, 'GETRAK-6DCT450','TN-G004', 2021, (SELECT Id FROM VehicleBrands WHERE Name='VW'), 235, 2, @Drive4Id, 'VW Tiguan 4Motion DSG'),
-- Peugeot
('TR-0013', @AisinId, 'AISIN-AT8',   'TN-A005', 2019, (SELECT Id FROM VehicleBrands WHERE Name='Peugeot'), 169, 2, @Drive2Id, 'Peugeot 2008 AT8'),
('TR-0014', @AisinId, 'AISIN-AT8',   'TN-A006', 2020, (SELECT Id FROM VehicleBrands WHERE Name='Peugeot'), 170, 2, @Drive2Id, 'Peugeot 3008 AT8'),
('TR-0015', @AisinId, 'AISIN-AT8',   'TN-A007', 2021, (SELECT Id FROM VehicleBrands WHERE Name='Peugeot'), 171, 1, @Drive2Id, 'Peugeot 5008 AT8'),
('TR-0016', @AisinId, 'AISIN-AT8',   'TN-A008', 2018, (SELECT Id FROM VehicleBrands WHERE Name='Peugeot'), 167, 3, @Drive2Id, 'Peugeot 308 AT8'),
-- Opel
('TR-0017', @AisinId, 'AISIN-TF60',  'TN-A009', 2017, (SELECT Id FROM VehicleBrands WHERE Name='Opel'), 160, 2, @Drive2Id, 'Opel Corsa AT'),
('TR-0018', @AisinId, 'AISIN-TF70',  'TN-A010', 2019, (SELECT Id FROM VehicleBrands WHERE Name='Opel'), 161, 1, @Drive2Id, 'Opel Astra AT'),
('TR-0019', @AisinId, 'AISIN-TF80',  'TN-A011', 2018, (SELECT Id FROM VehicleBrands WHERE Name='Opel'), 162, 2, @Drive2Id, 'Opel Insignia AT'),
('TR-0020', @AisinId, 'AISIN-TF70',  'TN-A012', 2020, (SELECT Id FROM VehicleBrands WHERE Name='Opel'), 165, 2, @Drive2Id, 'Opel Combo AT'),
-- Skoda
('TR-0021', @GetrakId, 'GETRAK-6DCT250','TN-G005', 2018, (SELECT Id FROM VehicleBrands WHERE Name='Skoda'), 206, 2, @Drive2Id, 'Skoda Octavia DSG'),
('TR-0022', @GetrakId, 'GETRAK-7DCL750','TN-G006', 2019, (SELECT Id FROM VehicleBrands WHERE Name='Skoda'), 207, 1, @Drive2Id, 'Skoda Superb DSG'),
('TR-0023', @GetrakId, 'GETRAK-6DCT450','TN-G007', 2020, (SELECT Id FROM VehicleBrands WHERE Name='Skoda'), 209, 2, @Drive2Id, 'Skoda Karoq DSG'),
('TR-0024', @GetrakId, 'GETRAK-7DCL750','TN-G008', 2021, (SELECT Id FROM VehicleBrands WHERE Name='Skoda'), 210, 2, @Drive4Id, 'Skoda Kodiaq 4x4 DSG'),
-- Nissan
('TR-0025', @JatcoId, 'JATCO-CVT8',  'TN-J005', 2018, (SELECT Id FROM VehicleBrands WHERE Name='Nissan'), 154, 2, @Drive2Id, 'Nissan Qashqai CVT'),
('TR-0026', @JatcoId, 'JATCO-CVT8',  'TN-J006', 2019, (SELECT Id FROM VehicleBrands WHERE Name='Nissan'), 155, 2, @Drive4Id, 'Nissan X-Trail 4x4 CVT'),
('TR-0027', @JatcoId, 'JATCO-CVT7',  'TN-J007', 2017, (SELECT Id FROM VehicleBrands WHERE Name='Nissan'), 156, 3, @Drive2Id, 'Nissan Juke CVT'),
('TR-0028', @JatcoId, 'JATCO-CVT8',  'TN-J008', 2020, (SELECT Id FROM VehicleBrands WHERE Name='Nissan'), 159, 2, @Drive2Id, 'Nissan NP300 AT'),
-- BMW
('TR-0029', @ZFId, 'ZF-8HP45',    'TN-Z001', 2018, (SELECT Id FROM VehicleBrands WHERE Name='BMW'), 28,  1, @Drive2Id, 'BMW 3 Serisi ZF8'),
('TR-0030', @ZFId, 'ZF-8HP70',    'TN-Z002', 2019, (SELECT Id FROM VehicleBrands WHERE Name='BMW'), 29,  2, @Drive2Id, 'BMW 5 Serisi ZF8'),
('TR-0031', @ZFId, 'ZF-8HP50X',   'TN-Z003', 2020, (SELECT Id FROM VehicleBrands WHERE Name='BMW'), 37,  2, @Drive4Id, 'BMW X3 xDrive ZF8'),
('TR-0032', @ZFId, 'ZF-8HP50X',   'TN-Z004', 2021, (SELECT Id FROM VehicleBrands WHERE Name='BMW'), 38,  1, @Drive4Id, 'BMW X5 xDrive ZF8'),
-- Mercedes
('TR-0033', @ZFId, 'ZF-9HP',      'TN-Z005', 2018, (SELECT Id FROM VehicleBrands WHERE Name='Mercedes'), 128, 2, @Drive2Id, 'Mercedes C-Serisi 9 ileri'),
('TR-0034', @ZFId, 'ZF-9HP',      'TN-Z006', 2019, (SELECT Id FROM VehicleBrands WHERE Name='Mercedes'), 129, 1, @Drive2Id, 'Mercedes E-Serisi 9 ileri'),
('TR-0035', @ZFId, 'ZF-9HPX',     'TN-Z007', 2020, (SELECT Id FROM VehicleBrands WHERE Name='Mercedes'), 132, 2, @Drive4Id, 'Mercedes GLA 4Matic'),
('TR-0036', @ZFId, 'ZF-9HPX',     'TN-Z008', 2021, (SELECT Id FROM VehicleBrands WHERE Name='Mercedes'), 134, 2, @Drive4Id, 'Mercedes GLC 4Matic'),
-- Audi
('TR-0037', @GetrakId, 'GETRAK-7DCT', 'TN-G009', 2018, (SELECT Id FROM VehicleBrands WHERE Name='Audi'), 10,  2, @Drive2Id, 'Audi A3 S tronic'),
('TR-0038', @GetrakId, 'GETRAK-7DCT', 'TN-G010', 2019, (SELECT Id FROM VehicleBrands WHERE Name='Audi'), 16,  1, @Drive2Id, 'Audi Q3 S tronic'),
('TR-0039', @GetrakId, 'GETRAK-7DCT', 'TN-G011', 2020, (SELECT Id FROM VehicleBrands WHERE Name='Audi'), 18,  2, @Drive4Id, 'Audi Q5 quattro S tronic'),
('TR-0040', @GetrakId, 'GETRAK-8AT',  'TN-G012', 2021, (SELECT Id FROM VehicleBrands WHERE Name='Audi'), 12,  2, @Drive2Id, 'Audi A6 otomatik');

----------------------------------------------------------------
-- 4) Raf Id’lerini oku (örnek: a1..c2)
----------------------------------------------------------------
DECLARE @a1 INT = (SELECT Id FROM StockLocations WHERE ShelfCode='a1');
DECLARE @a2 INT = (SELECT Id FROM StockLocations WHERE ShelfCode='a2');
DECLARE @b1 INT = (SELECT Id FROM StockLocations WHERE ShelfCode='b1');
DECLARE @b2 INT = (SELECT Id FROM StockLocations WHERE ShelfCode='b2');
DECLARE @c1 INT = (SELECT Id FROM StockLocations WHERE ShelfCode='c1');
DECLARE @c2 INT = (SELECT Id FROM StockLocations WHERE ShelfCode='c2');

----------------------------------------------------------------
-- 5) TransmissionStockLocations dağıt
----------------------------------------------------------------
INSERT INTO TransmissionStockLocations (TransmissionStockId, StockLocationId, Quantity)
SELECT
ts.Id,
CASE ts.Id % 6
WHEN 0 THEN @a1
WHEN 1 THEN @a2
WHEN 2 THEN @b1
WHEN 3 THEN @b2
WHEN 4 THEN @c1
ELSE      @c2
END AS StockLocationId,
CASE ts.Id % 4
WHEN 0 THEN 3
WHEN 1 THEN 4
WHEN 2 THEN 5
ELSE      4
END AS Quantity
FROM TransmissionStocks ts
WHERE ts.SparePartNo LIKE 'TR-00%';

-- Yarıya ikinci raf
INSERT INTO TransmissionStockLocations (TransmissionStockId, StockLocationId, Quantity)
SELECT
ts.Id,
CASE ((ts.Id % 6) + 1) % 6
WHEN 0 THEN @a1
WHEN 1 THEN @a2
WHEN 2 THEN @b1
WHEN 3 THEN @b2
WHEN 4 THEN @c1
ELSE      @c2
END AS StockLocationId,
CASE ts.Id % 3
WHEN 0 THEN 2
WHEN 1 THEN 1
ELSE      3
END AS Quantity
FROM TransmissionStocks ts
WHERE ts.SparePartNo LIKE 'TR-00%'
AND (ts.Id % 2) = 0;

COMMIT TRAN;
