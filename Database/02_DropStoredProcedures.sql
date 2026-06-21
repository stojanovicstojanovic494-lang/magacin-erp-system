-- ================================================
-- MAGACIN ERP SISTEM - OBRISATI SVE STORED PROCEDURES I VIEWS
-- SQL Server 2022
-- ================================================

USE MagacinERP
GO

-- Obrisati sve postojeće Stored Procedures
DROP PROCEDURE IF EXISTS sp_DodajPrijemu
DROP PROCEDURE IF EXISTS sp_DodajPrijemaStavku
DROP PROCEDURE IF EXISTS sp_AzurirajZaliheNakonPrijeme
DROP PROCEDURE IF EXISTS sp_DodajIzdavanje
DROP PROCEDURE IF EXISTS sp_SmanjiZalihe
DROP PROCEDURE IF EXISTS sp_DodajTransfer
DROP PROCEDURE IF EXISTS sp_DodajInventuru
DROP PROCEDURE IF EXISTS sp_GetDostupnaZaliha
DROP PROCEDURE IF EXISTS sp_GetZalihaPoLokacijama
DROP PROCEDURE IF EXISTS sp_GetArtikliSaNiskomZalijhom
DROP PROCEDURE IF EXISTS sp_GetVrednostZalihe
DROP PROCEDURE IF EXISTS sp_ValidacijaKorisnika
DROP PROCEDURE IF EXISTS sp_GetIstorijaTransakcija
DROP PROCEDURE IF EXISTS sp_GetDostupneLokacije
DROP PROCEDURE IF EXISTS sp_ABCAnaliza

-- Obrisati deljene utility procedure
DROP PROCEDURE IF EXISTS sp_LogAudit
DROP PROCEDURE IF EXISTS sp_AzurirajZalihe

-- Obrisati deljene views
DROP VIEW IF EXISTS vw_VrednostZalihe
GO

PRINT 'Svi Stored Procedures i Views su obrisani!'
