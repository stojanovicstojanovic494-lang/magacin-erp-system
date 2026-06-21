-- ================================================
-- MAGACIN ERP SISTEM - OBRISATI I PONOVNO KREIRATI
-- SQL Server 2022
-- ================================================

-- PRVO: Obrisati staru bazu ako postoji
USE master
GO

IF EXISTS (SELECT * FROM sys.databases WHERE name = 'MagacinERP')
BEGIN
    ALTER DATABASE MagacinERP SET SINGLE_USER WITH ROLLBACK IMMEDIATE
    DROP DATABASE MagacinERP
    PRINT 'Stara baza je obrisana!'
END
GO

-- DRUGO: Kreirati novu bazu
CREATE DATABASE MagacinERP
GO

PRINT 'Nova baza je kreirana!'
GO

USE MagacinERP
GO

-- ================================================
-- SVE TABELE - KOMPLETAN SETUP
-- ================================================

-- 1. JEDINICE MERE
CREATE TABLE JediniceMere (
    JedinicaMereID INT PRIMARY KEY IDENTITY(1,1),
    Naziv NVARCHAR(50) NOT NULL UNIQUE,
    Skracenica NVARCHAR(10) NOT NULL UNIQUE,
    Opis NVARCHAR(255),
    Aktivna BIT DEFAULT 1,
    DatumKreiranja DATETIME DEFAULT GETDATE(),
    DatumIzmene DATETIME DEFAULT GETDATE()
)
GO

-- 2. KATEGORIJE ARTIKALA
CREATE TABLE KategorijeArtikala (
    KategorijaID INT PRIMARY KEY IDENTITY(1,1),
    Naziv NVARCHAR(100) NOT NULL UNIQUE,
    Opis NVARCHAR(255),
    Aktivna BIT DEFAULT 1,
    DatumKreiranja DATETIME DEFAULT GETDATE(),
    DatumIzmene DATETIME DEFAULT GETDATE()
)
GO

-- 3. DOBAVLJACI
CREATE TABLE Dobavljaci (
    DobavljacID INT PRIMARY KEY IDENTITY(1,1),
    NazivFirme NVARCHAR(100) NOT NULL,
    KontaktOsoba NVARCHAR(100),
    Email NVARCHAR(100),
    Telefon NVARCHAR(20),
    Adresa NVARCHAR(255),
    Grad NVARCHAR(50),
    PostanskiBroj NVARCHAR(10),
    Drzava NVARCHAR(50),
    PIB NVARCHAR(20),
    Matični NVARCHAR(20),
    Aktivan BIT DEFAULT 1,
    DatumKreiranja DATETIME DEFAULT GETDATE(),
    DatumIzmene DATETIME DEFAULT GETDATE()
)
GO

-- 4. ZONE U MAGACINU
CREATE TABLE Zone (
    ZonaID INT PRIMARY KEY IDENTITY(1,1),
    Naziv NVARCHAR(50) NOT NULL UNIQUE,
    Opis NVARCHAR(255),
    Kapacitet INT,
    ZauzecaLokacija INT DEFAULT 0,
    Aktivna BIT DEFAULT 1,
    DatumKreiranja DATETIME DEFAULT GETDATE(),
    DatumIzmene DATETIME DEFAULT GETDATE()
)
GO

-- 5. SKLADISNE LOKACIJE
CREATE TABLE SkladisneLokacije (
    LokacijaID INT PRIMARY KEY IDENTITY(1,1),
    ZonaID INT NOT NULL,
    Red INT NOT NULL,
    Polica INT NOT NULL,
    Opis NVARCHAR(255),
    JeLiSlobodna BIT DEFAULT 1,
    Aktivna BIT DEFAULT 1,
    DatumKreiranja DATETIME DEFAULT GETDATE(),
    DatumIzmene DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (ZonaID) REFERENCES Zone(ZonaID),
    UNIQUE (ZonaID, Red, Polica)
)
GO

-- 6. ARTIKLI
CREATE TABLE Artikli (
    ArtikalID INT PRIMARY KEY IDENTITY(1,1),
    SifraArtikla NVARCHAR(50) NOT NULL UNIQUE,
    NazivArtikla NVARCHAR(150) NOT NULL,
    KategorijaID INT NOT NULL,
    JedinicaMereID INT NOT NULL,
    CenaKupovine DECIMAL(18,2),
    CenaProdaje DECIMAL(18,2),
    MinimalneStalje INT DEFAULT 10,
    MaksimalneStalje INT DEFAULT 1000,
    Tezina DECIMAL(10,3),
    Zapremina DECIMAL(10,3),
    Opis NVARCHAR(500),
    Barkod NVARCHAR(50),
    Aktivan BIT DEFAULT 1,
    DatumKreiranja DATETIME DEFAULT GETDATE(),
    DatumIzmene DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (KategorijaID) REFERENCES KategorijeArtikala(KategorijaID),
    FOREIGN KEY (JedinicaMereID) REFERENCES JediniceMere(JedinicaMereID)
)
GO

-- 7. ZALIHE
CREATE TABLE Zalihe (
    ZalihaID INT PRIMARY KEY IDENTITY(1,1),
    ArtikalID INT NOT NULL,
    LokacijaID INT NOT NULL,
    Kolicina INT NOT NULL DEFAULT 0,
    RezervovanoKolicina INT DEFAULT 0,
    DisponibilnaKolicina INT DEFAULT 0,
    DatumZadnjeIzmene DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (ArtikalID) REFERENCES Artikli(ArtikalID),
    FOREIGN KEY (LokacijaID) REFERENCES SkladisneLokacije(LokacijaID),
    UNIQUE (ArtikalID, LokacijaID)
)
GO

-- 8. LOTIBROJEVI
CREATE TABLE LotiBrojevi (
    LotID INT PRIMARY KEY IDENTITY(1,1),
    ArtikalID INT NOT NULL,
    LotBroj NVARCHAR(50) NOT NULL,
    SerijskiBroj NVARCHAR(50),
    DatumProizvodnje DATE,
    DatumRoka DATE,
    Kolicina INT NOT NULL,
    ZauzecaLokacija INT,
    Status NVARCHAR(20) DEFAULT 'Dostupan',
    DatumKreiranja DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (ArtikalID) REFERENCES Artikli(ArtikalID),
    UNIQUE (ArtikalID, LotBroj)
)
GO

-- 9. TIPOVI DOKUMENATA
CREATE TABLE TipoviDokumenata (
    TipDokumentaID INT PRIMARY KEY IDENTITY(1,1),
    Naziv NVARCHAR(50) NOT NULL UNIQUE,
    Skracenica NVARCHAR(5) NOT NULL,
    Opis NVARCHAR(255),
    Aktivan BIT DEFAULT 1
)
GO

-- 10. PRIJEMA MATERIJALA
CREATE TABLE PrijemaMaterijala (
    PrijemaID INT PRIMARY KEY IDENTITY(1,1),
    BrojDokumenta NVARCHAR(50) NOT NULL UNIQUE,
    DatumPrijeme DATETIME DEFAULT GETDATE(),
    DobavljacID INT NOT NULL,
    Napomena NVARCHAR(500),
    Status NVARCHAR(20) DEFAULT 'Otvorena',
    Korisnik NVARCHAR(100),
    DatumZavrsenja DATETIME,
    DatumKreiranja DATETIME DEFAULT GETDATE(),
    DatumIzmene DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (DobavljacID) REFERENCES Dobavljaci(DobavljacID)
)
GO

-- 11. PRIJEMA STAVKE
CREATE TABLE PrijemaStavke (
    PrijemaStavkaID INT PRIMARY KEY IDENTITY(1,1),
    PrijemaID INT NOT NULL,
    ArtikalID INT NOT NULL,
    KolicinaNarudjena INT NOT NULL,
    KolicinaPrimljena INT NOT NULL DEFAULT 0,
    CenaJedinice DECIMAL(18,2),
    Napomena NVARCHAR(255),
    LotBroj NVARCHAR(50),
    DatumRoka DATE,
    DatumKreiranja DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (PrijemaID) REFERENCES PrijemaMaterijala(PrijemaID),
    FOREIGN KEY (ArtikalID) REFERENCES Artikli(ArtikalID)
)
GO

-- 12. IZDAVANJE MATERIJALA
CREATE TABLE IzdavanjeMaterijala (
    IzdavanjeID INT PRIMARY KEY IDENTITY(1,1),
    BrojDokumenta NVARCHAR(50) NOT NULL UNIQUE,
    DatumIzdavanja DATETIME DEFAULT GETDATE(),
    TipIzdavanja NVARCHAR(50),
    Napomena NVARCHAR(500),
    Status NVARCHAR(20) DEFAULT 'Otvorena',
    Korisnik NVARCHAR(100),
    DatumZavrsenja DATETIME,
    DatumKreiranja DATETIME DEFAULT GETDATE(),
    DatumIzmene DATETIME DEFAULT GETDATE()
)
GO

-- 13. IZDAVANJE STAVKE
CREATE TABLE IzdavanjeStavke (
    IzdavanjeStavkaID INT PRIMARY KEY IDENTITY(1,1),
    IzdavanjeID INT NOT NULL,
    ArtikalID INT NOT NULL,
    KolicinaTrazena INT NOT NULL,
    KolicinaIzdata INT NOT NULL DEFAULT 0,
    CenaJedinice DECIMAL(18,2),
    Napomena NVARCHAR(255),
    LotID INT,
    DatumKreiranja DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (IzdavanjeID) REFERENCES IzdavanjeMaterijala(IzdavanjeID),
    FOREIGN KEY (ArtikalID) REFERENCES Artikli(ArtikalID),
    FOREIGN KEY (LotID) REFERENCES LotiBrojevi(LotID)
)
GO

-- 14. TRANSFERI
CREATE TABLE Transferi (
    TransferID INT PRIMARY KEY IDENTITY(1,1),
    BrojDokumenta NVARCHAR(50) NOT NULL UNIQUE,
    DatumTransfera DATETIME DEFAULT GETDATE(),
    IzLokacijeID INT NOT NULL,
    ULokacijuID INT NOT NULL,
    Napomena NVARCHAR(500),
    Status NVARCHAR(20) DEFAULT 'Otvorena',
    Korisnik NVARCHAR(100),
    DatumZavrsenja DATETIME,
    DatumKreiranja DATETIME DEFAULT GETDATE(),
    DatumIzmene DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (IzLokacijeID) REFERENCES SkladisneLokacije(LokacijaID),
    FOREIGN KEY (ULokacijuID) REFERENCES SkladisneLokacije(LokacijaID)
)
GO

-- 15. TRANSFERI STAVKE
CREATE TABLE TransferiStavke (
    TransferStavkaID INT PRIMARY KEY IDENTITY(1,1),
    TransferID INT NOT NULL,
    ArtikalID INT NOT NULL,
    Kolicina INT NOT NULL,
    LotID INT,
    Napomena NVARCHAR(255),
    DatumKreiranja DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (TransferID) REFERENCES Transferi(TransferID),
    FOREIGN KEY (ArtikalID) REFERENCES Artikli(ArtikalID),
    FOREIGN KEY (LotID) REFERENCES LotiBrojevi(LotID)
)
GO

-- 16. INVENTURE
CREATE TABLE Inventure (
    InventuraID INT PRIMARY KEY IDENTITY(1,1),
    BrojDokumenta NVARCHAR(50) NOT NULL UNIQUE,
    DatumInventure DATETIME DEFAULT GETDATE(),
    TipInventure NVARCHAR(20),
    Napomena NVARCHAR(500),
    Status NVARCHAR(20) DEFAULT 'U_toku',
    Korisnik NVARCHAR(100),
    DatumZavrsenja DATETIME,
    DatumKreiranja DATETIME DEFAULT GETDATE(),
    DatumIzmene DATETIME DEFAULT GETDATE()
)
GO

-- 17. INVENTURE STAVKE
CREATE TABLE InventureStavke (
    InventuraStavkaID INT PRIMARY KEY IDENTITY(1,1),
    InventuraID INT NOT NULL,
    LokacijaID INT NOT NULL,
    ArtikalID INT NOT NULL,
    UtvrdjenaKolicina INT NOT NULL,
    SistemskaBaza INT,
    Razlika INT,
    Napomena NVARCHAR(255),
    DatumKreiranja DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (InventuraID) REFERENCES Inventure(InventuraID),
    FOREIGN KEY (LokacijaID) REFERENCES SkladisneLokacije(LokacijaID),
    FOREIGN KEY (ArtikalID) REFERENCES Artikli(ArtikalID)
)
GO

-- 18. KOREKCIJE ZALIHA
CREATE TABLE KorrekcijeZaliha (
    KorrekcijaID INT PRIMARY KEY IDENTITY(1,1),
    BrojDokumenta NVARCHAR(50) NOT NULL UNIQUE,
    DatumKorekcije DATETIME DEFAULT GETDATE(),
    ArtikalID INT NOT NULL,
    LokacijaID INT NOT NULL,
    KolicinaStara INT,
    KolicinaNova INT,
    Razlog NVARCHAR(255),
    Napomena NVARCHAR(500),
    Korisnik NVARCHAR(100),
    DatumKreiranja DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (ArtikalID) REFERENCES Artikli(ArtikalID),
    FOREIGN KEY (LokacijaID) REFERENCES SkladisneLokacije(LokacijaID)
)
GO

-- 19. AUDIT LOG
CREATE TABLE AuditLog (
    AuditID INT PRIMARY KEY IDENTITY(1,1),
    Tabela NVARCHAR(100) NOT NULL,
    Akcija NVARCHAR(20),
    PrimarniKljuc NVARCHAR(100),
    StaraVrednost NVARCHAR(MAX),
    NovaVrednost NVARCHAR(MAX),
    Korisnik NVARCHAR(100),
    DatumAkcije DATETIME DEFAULT GETDATE()
)
GO

-- 20. KORISNICI
CREATE TABLE Korisnici (
    KorisnikID INT PRIMARY KEY IDENTITY(1,1),
    Korisnicko_Ime NVARCHAR(50) NOT NULL UNIQUE,
    LozinkaHash VARBINARY(64) NOT NULL,
    LozinkaSalt VARBINARY(32) NOT NULL,
    ImeKorisnika NVARCHAR(100) NOT NULL,
    Prezime NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100),
    Uloga NVARCHAR(50) CHECK (Uloga IN ('Admin', 'Magaciner', 'Pregled')),
    NeuspesniPokusaji INT DEFAULT 0,
    ZakljucanDo DATETIME NULL,
    Aktivan BIT DEFAULT 1,
    DatumKreiranja DATETIME DEFAULT GETDATE(),
    DatumIzmene DATETIME DEFAULT GETDATE()
)
GO

-- 21. DOZVOLE
CREATE TABLE Dozvole (
    DozvolaID INT PRIMARY KEY IDENTITY(1,1),
    KorisnikID INT NOT NULL,
    Opis NVARCHAR(100) NOT NULL,
    Aktivan BIT DEFAULT 1,
    FOREIGN KEY (KorisnikID) REFERENCES Korisnici(KorisnikID)
)
GO

-- 22. PARAMETRI SISTEMA
CREATE TABLE ParametriSistema (
    ParametarID INT PRIMARY KEY IDENTITY(1,1),
    NazivParametra NVARCHAR(100) NOT NULL UNIQUE,
    Vrednost NVARCHAR(MAX),
    TipParametra NVARCHAR(50),
    Opis NVARCHAR(255),
    DatumKreiranja DATETIME DEFAULT GETDATE(),
    DatumIzmene DATETIME DEFAULT GETDATE()
)
GO

-- ================================================
-- TEST PODACI
-- ================================================

INSERT INTO JediniceMere (Naziv, Skracenica, Opis) VALUES
('Komad', 'kom', 'Jedinična mera - komad'),
('Kilogram', 'kg', 'Težinska mera - kilogram'),
('Litar', 'l', 'Zapreminska mera - litar'),
('Metar', 'm', 'Dužinska mera - metar')
GO

INSERT INTO KategorijeArtikala (Naziv, Opis) VALUES
('Elektrotehnika', 'Električni elementi i komponente'),
('Mehanika', 'Mehanički delovi i sklopovi'),
('Hemikalije', 'Hemijske supstance i materijali'),
('Tekstil', 'Tekstilni proizvodi')
GO

INSERT INTO Dobavljaci (NazivFirme, KontaktOsoba, Email, Telefon, Adresa, Grad, PostanskiBroj, Drzava) VALUES
('ELEKTRO d.o.o.', 'Marko Marković', 'marko@elektro.rs', '+381 11 123 4567', 'Bulevar oslobođenja 50', 'Beograd', '11000', 'Srbija'),
('METAL Imports', 'Jovana Jovanović', 'jovana@metal.rs', '+381 21 987 6543', 'Ulica Francuska 15', 'Novi Sad', '21000', 'Srbija')
GO

INSERT INTO Zone (Naziv, Opis, Kapacitet) VALUES
('ZONA A - Elektronika', 'Zona za električne komponente', 500),
('ZONA B - Metali', 'Zona za metalne delove', 300)
GO

INSERT INTO SkladisneLokacije (ZonaID, Red, Polica, Opis) VALUES
(1, 1, 1, 'Polica A1-1'),
(1, 1, 2, 'Polica A1-2'),
(2, 1, 1, 'Polica B1-1'),
(2, 1, 2, 'Polica B1-2')
GO

INSERT INTO Artikli (SifraArtikla, NazivArtikla, KategorijaID, JedinicaMereID, CenaKupovine, CenaProdaje, MinimalneStalje, MaksimalneStalje, Opis) VALUES
('EL001', 'Kondenzator 100μF', 1, 1, 50.00, 75.00, 20, 200, 'Elektrolitski kondenzator'),
('EL002', 'Otpornik 1kΩ', 1, 1, 10.00, 15.00, 100, 500, 'Film otpornik'),
('ME001', 'Vijak M8x20', 2, 1, 2.50, 4.00, 200, 1000, 'Metalni vijak'),
('ME002', 'Matica M8', 2, 1, 1.50, 2.50, 300, 1500, 'Metalna matica')
GO

INSERT INTO Zalihe (ArtikalID, LokacijaID, Kolicina, DisponibilnaKolicina) VALUES
(1, 1, 100, 100),
(2, 2, 250, 250),
(3, 3, 500, 500),
(4, 4, 800, 800)
GO

-- Helper function for password hashing
CREATE FUNCTION dbo.fn_HashPassword(
    @Lozinka NVARCHAR(255),
    @Salt VARBINARY(32)
)
RETURNS VARBINARY(64)
AS
BEGIN
    RETURN HASHBYTES('SHA2_256', CONCAT(CAST(@Salt AS NVARCHAR(MAX)), @Lozinka))
END
GO

DECLARE @Salt1 VARBINARY(32) = 0x0102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F20
DECLARE @Salt2 VARBINARY(32) = 0x2122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F40
DECLARE @Salt3 VARBINARY(32) = 0x4142434445464748494A4B4C4D4E4F505152535455565758595A5B5C5D5E5F60

INSERT INTO Korisnici (Korisnicko_Ime, LozinkaHash, LozinkaSalt, ImeKorisnika, Prezime, Email, Uloga, Aktivan) VALUES
('admin',     dbo.fn_HashPassword('admin123', @Salt1),     @Salt1, 'Administratski', 'Korisnik', 'admin@magacin.rs', 'Admin', 1),
('magaciner', dbo.fn_HashPassword('magacin123', @Salt2),   @Salt2, 'Marko', 'Marković', 'marko@magacin.rs', 'Magaciner', 1),
('pregled',   dbo.fn_HashPassword('pregled123', @Salt3),   @Salt3, 'Pregleda', 'Korisnik', 'pregled@magacin.rs', 'Pregled', 1)
GO

-- ================================================
-- INDEKSI
-- ================================================
CREATE INDEX IDX_Artikli_Sifra ON Artikli(SifraArtikla)
CREATE INDEX IDX_Artikli_Naziv ON Artikli(NazivArtikla)
CREATE INDEX IDX_Zalihe_Artikal ON Zalihe(ArtikalID)
CREATE INDEX IDX_Zalihe_Lokacija ON Zalihe(LokacijaID)
GO

PRINT '================================'
PRINT 'Baza podataka je uspešno kreirana!'
PRINT '================================'
PRINT 'Test korisnici su kreirani (videti dokumentaciju za lozinke).'
