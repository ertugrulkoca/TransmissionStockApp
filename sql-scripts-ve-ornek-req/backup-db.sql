USE master;
GO

RESTORE FILELISTONLY
FROM DISK = N'C:\Backups\TransmissionStockDb_FULL.bak';
GO


USE master;
GO

RESTORE DATABASE [TransmissionStockDb]
FROM DISK = N'C:\Backups\TransmissionStockDb_FULL.bak'
WITH
    MOVE N'TransmissionStockDb'     TO N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\TransmissionStockDb.mdf',
    MOVE N'TransmissionStockDb_log' TO N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\TransmissionStockDb_log.ldf',
    RECOVERY,
    REPLACE,
    STATS = 10;
GO
