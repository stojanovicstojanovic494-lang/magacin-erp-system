-- ================================================
-- MAGACIN ERP - BRISANJE STARE BAZE
-- SQL Server 2022
-- ================================================

USE master
GO

-- Prvo: Zatvori sve konekcije ka starijoj bazi
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'MagacinERP')
BEGIN
    ALTER DATABASE MagacinERP SET SINGLE_USER WITH ROLLBACK IMMEDIATE
    DROP DATABASE MagacinERP
    PRINT 'Stara baza je obrisana!'
END
GO

-- Pauza od 2 sekunde
WAITFOR DELAY '00:00:02'
GO

-- Kreiraj novu bazu
CREATE DATABASE MagacinERP
GO

PRINT 'Nova baza je kreirana!'
