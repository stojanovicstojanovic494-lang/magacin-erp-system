-- ================================================
-- MAGACIN ERP SISTEM - DATABASE SETUP
-- SQL Server 2022
-- ================================================

-- Kreiramo bazu podataka
CREATE DATABASE MagacinERP
GO

USE MagacinERP
GO

-- ================================================
-- 1. TABELA ZA JEDINICE MERE
-- ================================================
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

-- ================================================
-- 2. TABELA ZA KATEGORIJE ARTIKALA
-- ================================================
CREATE TABLE KategorijeArtikala (
    KategorijaID INT PRIMARY KEY IDENTITY(1,1),
    Naziv NVARCHAR(100) NOT NULL UNIQUE,
    Opis NVARCHAR(255),
    Aktivna BIT DEFAULT 1,
    DatumKreiranja DATETIME DEFAULT GETDATE(),
    DatumIzmene DATETIME DEFAULT GETDATE()
)
GO

-- ================================================
-- 3. TABELA ZA DOBAVLJACE
-- ================================================
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

-- ================================================
-- 4. TABELA ZA ZONE U MAGACINU
-- ================================================
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

-- ================================================
-- 5. TABELA ZA SKLADISNE LOKACIJE
-- ================================================
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

-- ================================================
-- 6. TABELA ZA ARTIKLE
-- ================================================
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

-- ================================================
-- 7. TABELA ZA ZALIHE
-- ================================================
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

-- ================================================
-- 8. TABELA ZA LOT / SERIJSKE BROJEVE
-- ================================================
CREATE TABLE LotiBrojevi (
    LotID INT PRIMARY KEY IDENTITY(1,1),
    ArtikalID INT NOT NULL,
    LotBroj NVARCHAR(50) NOT NULL,
    SerijskiBroj NVARCHAR(50),
    DatumProizvodnje DATE,
    DatumRoka DATE,
    Kolicina INT NOT NULL,
    ZauzecaLokacija INT,
    Status NVARCHAR(20) DEFAULT 'Dostupan' CHECK (Status IN ('Dostupan', 'Rezervovan', 'Izdat', 'Istekao')),
    DatumKreiranja DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (ArtikalID) REFERENCES Artikli(ArtikalID),
    UNIQUE (ArtikalID, LotBroj)
)
GO

-- ================================================
-- 9. TABELA ZA TIPOVE DOKUMENATA
-- ================================================
CREATE TABLE TipoviDokumenata (
    TipDokumentaID INT PRIMARY KEY IDENTITY(1,1),
    Naziv NVARCHAR(50) NOT NULL UNIQUE,
    Skracenica NVARCHAR(5) NOT NULL,
    Opis NVARCHAR(255),
    Aktivan BIT DEFAULT 1
)
GO

-- ================================================
-- 10. TABELA ZA TRANSAKCIE - GRN (GOODS RECEIPT NOTE)
-- ================================================
CREATE TABLE PrijemaMaterijala (
    PrijemaID INT PRIMARY KEY IDENTITY(1,1),
    BrojDokumenta NVARCHAR(50) NOT NULL UNIQUE,
    DatumPrijeme DATETIME DEFAULT GETDATE(),
    DobavljacID INT NOT NULL,
    Napomena NVARCHAR(500),
    Status NVARCHAR(20) DEFAULT 'Otvorena' CHECK (Status IN ('Otvorena', 'Zavrsena', 'Otkazana')),
    Korisnik NVARCHAR(100),
    DatumZavrsenja DATETIME,
    DatumKreiranja DATETIME DEFAULT GETDATE(),
    DatumIzmene DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (DobavljacID) REFERENCES Dobavljaci(DobavljacID)
)
GO

-- ================================================
-- 11. TABELA ZA STAVKE PRIJEME
-- ================================================
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

-- ================================================
-- 12. TABELA ZA IZDAVANJE MATERIJALA
-- ================================================
CREATE TABLE IzdavanjeMaterijala (
    IzdavanjeID INT PRIMARY KEY IDENTITY(1,1),
    BrojDokumenta NVARCHAR(50) NOT NULL UNIQUE,
    DatumIzdavanja DATETIME DEFAULT GETDATE(),
    TipIzdavanja NVARCHAR(50), -- Proizvodnja, Prodaja, Povracaj, Transfer
    Napomena NVARCHAR(500),
    Status NVARCHAR(20) DEFAULT 'Otvorena' CHECK (Status IN ('Otvorena', 'Zavrsena', 'Otkazana')),
    Korisnik NVARCHAR(100),
    DatumZavrsenja DATETIME,
    DatumKreiranja DATETIME DEFAULT GETDATE(),
    DatumIzmene DATETIME DEFAULT GETDATE()
)
GO

-- ================================================
-- 13. TABELA ZA STAVKE IZDAVANJA
-- ================================================
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

-- ================================================
-- 14. TABELA ZA TRANSFERE IZMEĐU LOKACIJA
-- ================================================
CREATE TABLE Transferi (
    TransferID INT PRIMARY KEY IDENTITY(1,1),
    BrojDokumenta NVARCHAR(50) NOT NULL UNIQUE,
    DatumTransfera DATETIME DEFAULT GETDATE(),
    IzLokacijeID INT NOT NULL,
    ULokacijuID INT NOT NULL,
    Napomena NVARCHAR(500),
    Status NVARCHAR(20) DEFAULT 'Otvorena' CHECK (Status IN ('Otvorena', 'Zavrsena', 'Otkazana')),
    Korisnik NVARCHAR(100),
    DatumZavrsenja DATETIME,
    DatumKreiranja DATETIME DEFAULT GETDATE(),
    DatumIzmene DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (IzLokacijeID) REFERENCES SkladisneLokacije(LokacijaID),
    FOREIGN KEY (ULokacijuID) REFERENCES SkladisneLokacije(LokacijaID)
)
GO

-- ================================================
-- 15. TABELA ZA STAVKE TRANSFERA
-- ================================================
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

-- ================================================
-- 16. TABELA ZA INVENTURU (PREBROJAVANJE)
-- ================================================
CREATE TABLE Inventure (
    InventuraID INT PRIMARY KEY IDENTITY(1,1),
    BrojDokumenta NVARCHAR(50) NOT NULL UNIQUE,
    DatumInventure DATETIME DEFAULT GETDATE(),
    TipInventure NVARCHAR(20), -- Parcijalna, Potpuna
    Napomena NVARCHAR(500),
    Status NVARCHAR(20) DEFAULT 'U_toku' CHECK (Status IN ('U_toku', 'Zavrsena', 'Otkazana')),
    Korisnik NVARCHAR(100),
    DatumZavrsenja DATETIME,
    DatumKreiranja DATETIME DEFAULT GETDATE(),
    DatumIzmene DATETIME DEFAULT GETDATE()
)
GO

-- ================================================
-- 17. TABELA ZA STAVKE INVENTURE
-- ================================================
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

-- ================================================
-- 18. TABELA ZA KOREKCIJE ZALIHA
-- ================================================
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

-- ================================================
-- 19. TABELA ZA AUDIT LOG
-- ================================================
CREATE TABLE AuditLog (
    AuditID INT PRIMARY KEY IDENTITY(1,1),
    Tabela NVARCHAR(100) NOT NULL,
    Akcija NVARCHAR(20), -- INSERT, UPDATE, DELETE
    PrimarniKljuc NVARCHAR(100),
    StaraVrednost NVARCHAR(MAX),
    NovaVrednost NVARCHAR(MAX),
    Korisnik NVARCHAR(100),
    DatumAkcije DATETIME DEFAULT GETDATE()
)
GO

-- ================================================
-- 20. TABELA ZA KORISNIKE
-- ================================================
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

-- ================================================
-- 21. TABELA ZA DOZVOLE KORISNIKA
-- ================================================
CREATE TABLE Dozvole (
    DozvolaID INT PRIMARY KEY IDENTITY(1,1),
    KorisnikID INT NOT NULL,
    Opis NVARCHAR(100) NOT NULL,
    Aktivan BIT DEFAULT 1,
    FOREIGN KEY (KorisnikID) REFERENCES Korisnici(KorisnikID)
)
GO

-- ================================================
-- 22. TABELA ZA PARAMETRE SISTEMA
-- ================================================
CREATE TABLE ParametriSistema (
    ParametarID INT PRIMARY KEY IDENTITY(1,1),
    NazivParametra NVARCHAR(100) NOT NULL UNIQUE,
    Vrednost NVARCHAR(MAX),
    TipParametra NVARCHAR(50), -- String, Int, Decimal, Bool
    Opis NVARCHAR(255),
    DatumKreiranja DATETIME DEFAULT GETDATE(),
    DatumIzmene DATETIME DEFAULT GETDATE()
)
GO

-- ================================================
-- INDEKSI ZA PERFORMANSE
-- ================================================

CREATE INDEX IDX_Artikli_Sifra ON Artikli(SifraArtikla)
CREATE INDEX IDX_Artikli_Naziv ON Artikli(NazivArtikla)
CREATE INDEX IDX_Zalihe_Artikal ON Zalihe(ArtikalID)
CREATE INDEX IDX_Zalihe_Lokacija ON Zalihe(LokacijaID)
CREATE INDEX IDX_SkladisneLokacije_Zona ON SkladisneLokacije(ZonaID)
CREATE INDEX IDX_PrijemaMaterijala_Status ON PrijemaMaterijala(Status)
CREATE INDEX IDX_IzdavanjeMaterijala_Status ON IzdavanjeMaterijala(Status)
CREATE INDEX IDX_LotiBrojevi_Artikal ON LotiBrojevi(ArtikalID)
CREATE INDEX IDX_AuditLog_Datum ON AuditLog(DatumAkcije)
CREATE INDEX IDX_Korisnici_Korisnicko_Ime ON Korisnici(Korisnicko_Ime)

GO

-- ================================================
-- HELPER FUNCTION: HASH PASSWORD WITH SALT
-- ================================================
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

PRINT 'Baza podataka je uspešno kreirana!'
