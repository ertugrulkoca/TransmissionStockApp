BEGIN TRAN;

DECLARE @StockId INT;

--------------------------------------------------------------------------------
-- 1
INSERT INTO TransmissionStocks
(SparePartNo, TransmissionBrandId, TransmissionCode, TransmissionNumber, Year,
 VehicleBrandId, VehicleModelId, TransmissionDriveTypeId, TransmissionStatusId, Description)
VALUES
('TRS-TEST-0001', 1, 'ZF-8HP', 'ZF0001', 2018, 34, 232, 4, 3, N'Test kayıt 1 (VW Passat, 4 çeker, çıkma)');
SET @StockId = SCOPE_IDENTITY();
INSERT INTO TransmissionStockLocations (TransmissionStockId, ShelfId, Quantity) VALUES (@StockId, 5, 2);  -- Depo1/b1

--------------------------------------------------------------------------------
-- 2
INSERT INTO TransmissionStocks
(SparePartNo, TransmissionBrandId, TransmissionCode, TransmissionNumber, Year,
 VehicleBrandId, VehicleModelId, TransmissionDriveTypeId, TransmissionStatusId, Description)
VALUES
('TRS-TEST-0002', 1, 'ZF-6HP', 'ZF0002', 2016, 34, 231, 2, 2, N'Test kayıt 2 (VW Golf, 2 çeker, çıkma temiz)');
SET @StockId = SCOPE_IDENTITY();
INSERT INTO TransmissionStockLocations (TransmissionStockId, ShelfId, Quantity) VALUES (@StockId, 11, 1); -- Depo2/b2

--------------------------------------------------------------------------------
-- 3
INSERT INTO TransmissionStocks
(SparePartNo, TransmissionBrandId, TransmissionCode, TransmissionNumber, Year,
 VehicleBrandId, VehicleModelId, TransmissionDriveTypeId, TransmissionStatusId, Description)
VALUES
('TRS-TEST-0003', 2, 'AISIN-TF80', 'AI0003', 2017, 32, 219, 2, 3, N'Test kayıt 3 (Toyota Corolla, 2 çeker, çıkma)');
SET @StockId = SCOPE_IDENTITY();
INSERT INTO TransmissionStockLocations (TransmissionStockId, ShelfId, Quantity) VALUES (@StockId, 3, 3);  -- Depo1/a2
INSERT INTO TransmissionStockLocations (TransmissionStockId, ShelfId, Quantity) VALUES (@StockId, 10, 1); -- Depo2/b1

--------------------------------------------------------------------------------
-- 4
INSERT INTO TransmissionStocks
(SparePartNo, TransmissionBrandId, TransmissionCode, TransmissionNumber, Year,
 VehicleBrandId, VehicleModelId, TransmissionDriveTypeId, TransmissionStatusId, Description)
VALUES
('TRS-TEST-0004', 2, 'AISIN-U660', 'AI0004', 2015, 32, 224, 2, 1, N'Test kayıt 4 (Toyota Yaris, 2 çeker, sıfır)');
SET @StockId = SCOPE_IDENTITY();
INSERT INTO TransmissionStockLocations (TransmissionStockId, ShelfId, Quantity) VALUES (@StockId, 8, 2);  -- Depo2/a2

--------------------------------------------------------------------------------
-- 5
INSERT INTO TransmissionStocks
(SparePartNo, TransmissionBrandId, TransmissionCode, TransmissionNumber, Year,
 VehicleBrandId, VehicleModelId, TransmissionDriveTypeId, TransmissionStatusId, Description)
VALUES
('TRS-TEST-0005', 3, 'JATCO-CVT7', 'JT0005', 2019, 23, 154, 2, 2, N'Test kayıt 5 (Nissan Qashqai, 2 çeker, çıkma temiz)');
SET @StockId = SCOPE_IDENTITY();
INSERT INTO TransmissionStockLocations (TransmissionStockId, ShelfId, Quantity) VALUES (@StockId, 6, 1);  -- Depo1/b2

--------------------------------------------------------------------------------
-- 6
INSERT INTO TransmissionStocks
(SparePartNo, TransmissionBrandId, TransmissionCode, TransmissionNumber, Year,
 VehicleBrandId, VehicleModelId, TransmissionDriveTypeId, TransmissionStatusId, Description)
VALUES
('TRS-TEST-0006', 3, 'JATCO-CVT8', 'JT0006', 2020, 23, 155, 4, 3, N'Test kayıt 6 (Nissan X-Trail, 4 çeker, çıkma)');
SET @StockId = SCOPE_IDENTITY();
INSERT INTO TransmissionStockLocations (TransmissionStockId, ShelfId, Quantity) VALUES (@StockId, 12, 4); -- Depo2/b3
INSERT INTO TransmissionStockLocations (TransmissionStockId, ShelfId, Quantity) VALUES (@StockId, 4, 1);  -- Depo1/a3

--------------------------------------------------------------------------------
-- 7
INSERT INTO TransmissionStocks
(SparePartNo, TransmissionBrandId, TransmissionCode, TransmissionNumber, Year,
 VehicleBrandId, VehicleModelId, TransmissionDriveTypeId, TransmissionStatusId, Description)
VALUES
('TRS-TEST-0007', 4, 'GETRAG-DCT', 'GT0007', 2018, 5, 28, 2, 3, N'Test kayıt 7 (BMW 3 Serisi, 2 çeker, çıkma)');
SET @StockId = SCOPE_IDENTITY();
INSERT INTO TransmissionStockLocations (TransmissionStockId, ShelfId, Quantity) VALUES (@StockId, 9, 2);  -- Depo2/a3

--------------------------------------------------------------------------------
-- 8
INSERT INTO TransmissionStocks
(SparePartNo, TransmissionBrandId, TransmissionCode, TransmissionNumber, Year,
 VehicleBrandId, VehicleModelId, TransmissionDriveTypeId, TransmissionStatusId, Description)
VALUES
('TRS-TEST-0008', 4, 'GETRAG-6MT', 'GT0008', 2017, 5, 36, 4, 2, N'Test kayıt 8 (BMW X1, 4 çeker, çıkma temiz)');
SET @StockId = SCOPE_IDENTITY();
INSERT INTO TransmissionStockLocations (TransmissionStockId, ShelfId, Quantity) VALUES (@StockId, 1, 1);  -- Depo1/a1

--------------------------------------------------------------------------------
-- 9
INSERT INTO TransmissionStocks
(SparePartNo, TransmissionBrandId, TransmissionCode, TransmissionNumber, Year,
 VehicleBrandId, VehicleModelId, TransmissionDriveTypeId, TransmissionStatusId, Description)
VALUES
('TRS-TEST-0009', 1, 'ZF-9HP', 'ZF0009', 2019, 39, 293, 2, 4, N'Test kayıt 9 (Honda Civic, 2 çeker, arızalı)');
SET @StockId = SCOPE_IDENTITY();
INSERT INTO TransmissionStockLocations (TransmissionStockId, ShelfId, Quantity) VALUES (@StockId, 7, 1);  -- Depo1/b3

--------------------------------------------------------------------------------
-- 10
INSERT INTO TransmissionStocks
(SparePartNo, TransmissionBrandId, TransmissionCode, TransmissionNumber, Year,
 VehicleBrandId, VehicleModelId, TransmissionDriveTypeId, TransmissionStatusId, Description)
VALUES
('TRS-TEST-0010', 2, 'AISIN-AW55', 'AI0010', 2014, 35, 246, 2, 3, N'Test kayıt 10 (Volvo XC40, 2 çeker, çıkma)');
SET @StockId = SCOPE_IDENTITY();
INSERT INTO TransmissionStockLocations (TransmissionStockId, ShelfId, Quantity) VALUES (@StockId, 2, 2);  -- Depo2/a1

--------------------------------------------------------------------------------
-- 11
INSERT INTO TransmissionStocks
(SparePartNo, TransmissionBrandId, TransmissionCode, TransmissionNumber, Year,
 VehicleBrandId, VehicleModelId, TransmissionDriveTypeId, TransmissionStatusId, Description)
VALUES
('TRS-TEST-0011', 3, 'JATCO-RE0F', 'JT0011', 2016, 38, 281, 2, 2, N'Test kayıt 11 (Hyundai i30, 2 çeker, çıkma temiz)');
SET @StockId = SCOPE_IDENTITY();
INSERT INTO TransmissionStockLocations (TransmissionStockId, ShelfId, Quantity) VALUES (@StockId, 10, 3); -- Depo2/b1
INSERT INTO TransmissionStockLocations (TransmissionStockId, ShelfId, Quantity) VALUES (@StockId, 3, 2);  -- Depo1/a2

--------------------------------------------------------------------------------
-- 12
INSERT INTO TransmissionStocks
(SparePartNo, TransmissionBrandId, TransmissionCode, TransmissionNumber, Year,
 VehicleBrandId, VehicleModelId, TransmissionDriveTypeId, TransmissionStatusId, Description)
VALUES
('TRS-TEST-0012', 4, 'GETRAG-7DCT', 'GT0012', 2021, 20, 132, 4, 1, N'Test kayıt 12 (Mercedes GLA, 4 çeker, sıfır)');
SET @StockId = SCOPE_IDENTITY();
INSERT INTO TransmissionStockLocations (TransmissionStockId, ShelfId, Quantity) VALUES (@StockId, 8, 1);  -- Depo2/a2

--------------------------------------------------------------------------------
-- 13
INSERT INTO TransmissionStocks
(SparePartNo, TransmissionBrandId, TransmissionCode, TransmissionNumber, Year,
 VehicleBrandId, VehicleModelId, TransmissionDriveTypeId, TransmissionStatusId, Description)
VALUES
('TRS-TEST-0013', 2, 'AISIN-8AT', 'AI0013', 2022, 33, 228, 4, 3, N'Test kayıt 13 (TOGG SUV, 4 çeker, çıkma)');
SET @StockId = SCOPE_IDENTITY();
INSERT INTO TransmissionStockLocations (TransmissionStockId, ShelfId, Quantity) VALUES (@StockId, 11, 2); -- Depo2/b2

--------------------------------------------------------------------------------
-- 14
INSERT INTO TransmissionStocks
(SparePartNo, TransmissionBrandId, TransmissionCode, TransmissionNumber, Year,
 VehicleBrandId, VehicleModelId, TransmissionDriveTypeId, TransmissionStatusId, Description)
VALUES
('TRS-TEST-0014', 1, 'ZF-8HP45', 'ZF0014', 2018, 34, 235, 4, 2, N'Test kayıt 14 (VW Tiguan, 4 çeker, çıkma temiz)');
SET @StockId = SCOPE_IDENTITY();
INSERT INTO TransmissionStockLocations (TransmissionStockId, ShelfId, Quantity) VALUES (@StockId, 4, 3);  -- Depo1/a3

--------------------------------------------------------------------------------
-- 15
INSERT INTO TransmissionStocks
(SparePartNo, TransmissionBrandId, TransmissionCode, TransmissionNumber, Year,
 VehicleBrandId, VehicleModelId, TransmissionDriveTypeId, TransmissionStatusId, Description)
VALUES
('TRS-TEST-0015', 3, 'JATCO-CVT9', 'JT0015', 2017, 24, 161, 2, 3, N'Test kayıt 15 (Opel Astra, 2 çeker, çıkma)');
SET @StockId = SCOPE_IDENTITY();
INSERT INTO TransmissionStockLocations (TransmissionStockId, ShelfId, Quantity) VALUES (@StockId, 6, 2);  -- Depo1/b2
INSERT INTO TransmissionStockLocations (TransmissionStockId, ShelfId, Quantity) VALUES (@StockId, 9, 1);  -- Depo2/a3

--------------------------------------------------------------------------------
-- 16
INSERT INTO TransmissionStocks
(SparePartNo, TransmissionBrandId, TransmissionCode, TransmissionNumber, Year,
 VehicleBrandId, VehicleModelId, TransmissionDriveTypeId, TransmissionStatusId, Description)
VALUES
('TRS-TEST-0016', 4, 'GETRAG-8DCT', 'GT0016', 2020, 32, 221, 2, 4, N'Test kayıt 16 (Toyota C-HR, 2 çeker, arızalı)');
SET @StockId = SCOPE_IDENTITY();
INSERT INTO TransmissionStockLocations (TransmissionStockId, ShelfId, Quantity) VALUES (@StockId, 12, 1); -- Depo2/b3

--------------------------------------------------------------------------------
-- 17
INSERT INTO TransmissionStocks
(SparePartNo, TransmissionBrandId, TransmissionCode, TransmissionNumber, Year,
 VehicleBrandId, VehicleModelId, TransmissionDriveTypeId, TransmissionStatusId, Description)
VALUES
('TRS-TEST-0017', 1, 'ZF-6HP19', 'ZF0017', 2015, 5, 29, 2, 2, N'Test kayıt 17 (BMW 5 Serisi, 2 çeker, çıkma temiz)');
SET @StockId = SCOPE_IDENTITY();
INSERT INTO TransmissionStockLocations (TransmissionStockId, ShelfId, Quantity) VALUES (@StockId, 5, 4);  -- Depo1/b1

--------------------------------------------------------------------------------
-- 18
INSERT INTO TransmissionStocks
(SparePartNo, TransmissionBrandId, TransmissionCode, TransmissionNumber, Year,
 VehicleBrandId, VehicleModelId, TransmissionDriveTypeId, TransmissionStatusId, Description)
VALUES
('TRS-TEST-0018', 2, 'AISIN-TG81', 'AI0018', 2021, 38, 285, 4, 3, N'Test kayıt 18 (Hyundai Tucson, 4 çeker, çıkma)');
SET @StockId = SCOPE_IDENTITY();
INSERT INTO TransmissionStockLocations (TransmissionStockId, ShelfId, Quantity) VALUES (@StockId, 2, 2);  -- Depo2/a1
INSERT INTO TransmissionStockLocations (TransmissionStockId, ShelfId, Quantity) VALUES (@StockId, 7, 1);  -- Depo1/b3

--------------------------------------------------------------------------------
-- 19
INSERT INTO TransmissionStocks
(SparePartNo, TransmissionBrandId, TransmissionCode, TransmissionNumber, Year,
 VehicleBrandId, VehicleModelId, TransmissionDriveTypeId, TransmissionStatusId, Description)
VALUES
('TRS-TEST-0019', 3, 'JATCO-6AT', 'JT0019', 2016, 37, 266, 2, 1, N'Test kayıt 19 (Ford Focus, 2 çeker, sıfır)');
SET @StockId = SCOPE_IDENTITY();
INSERT INTO TransmissionStockLocations (TransmissionStockId, ShelfId, Quantity) VALUES (@StockId, 8, 1);  -- Depo2/a2

--------------------------------------------------------------------------------
-- 20
INSERT INTO TransmissionStocks
(SparePartNo, TransmissionBrandId, TransmissionCode, TransmissionNumber, Year,
 VehicleBrandId, VehicleModelId, TransmissionDriveTypeId, TransmissionStatusId, Description)
VALUES
('TRS-TEST-0020', 4, 'GETRAG-MT6', 'GT0020', 2019, 25, 166, 2, 3, N'Test kayıt 20 (Peugeot 208, 2 çeker, çıkma)');
SET @StockId = SCOPE_IDENTITY();
INSERT INTO TransmissionStockLocations (TransmissionStockId, ShelfId, Quantity) VALUES (@StockId, 1, 2);  -- Depo1/a1
INSERT INTO TransmissionStockLocations (TransmissionStockId, ShelfId, Quantity) VALUES (@StockId, 11, 1); -- Depo2/b2

COMMIT;
