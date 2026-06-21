-- ================================================
-- MAGACIN ERP SISTEM - OBRISATI I PONOVNO KREIRATI
-- SQL Server 2022
-- ================================================
-- Koristi SQLCMD mod za pokretanje:
--   sqlcmd -S server -i 00_DropAndRecreate.sql
--
-- Ovaj skript je tanak wrapper koji referencira
-- pojedinačne skripte umesto da duplira njihov sadržaj.
-- ================================================

-- KORAK 1: Obrisati staru bazu ako postoji
USE master
GO

IF EXISTS (SELECT * FROM sys.databases WHERE name = 'MagacinERP')
BEGIN
    ALTER DATABASE MagacinERP SET SINGLE_USER WITH ROLLBACK IMMEDIATE
    DROP DATABASE MagacinERP
    PRINT 'Stara baza je obrisana!'
END
GO

WAITFOR DELAY '00:00:02'
GO

-- KORAK 2: Kreirati bazu i sve tabele
:r 01_Create_Database.sql

-- KORAK 3: Učitati test podatke
:r 02_Insert_TestData.sql

-- KORAK 4: Obrisati stare stored procedures
:r 02_DropStoredProcedures.sql

-- KORAK 5: Kreirati stored procedures
:r 03_StoredProcedures.sql

PRINT '================================'
PRINT 'Baza podataka je potpuno resetovana!'
PRINT '================================'
