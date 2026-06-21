-- ================================================
-- MAGACIN ERP SISTEM - STORED PROCEDURES
-- SQL Server 2022
-- ================================================

USE MagacinERP
GO

-- ================================================
-- DELJENI UTILITY: AUDIT LOG
-- Zamenjuje 7x duplirani INSERT INTO AuditLog blok
-- ================================================
CREATE PROCEDURE sp_LogAudit
    @Tabela NVARCHAR(100),
    @Akcija NVARCHAR(20),
    @PrimarniKljuc NVARCHAR(100),
    @NovaVrednost NVARCHAR(MAX),
    @Korisnik NVARCHAR(100)
AS
BEGIN
    INSERT INTO AuditLog (Tabela, Akcija, PrimarniKljuc, NovaVrednost, Korisnik)
    VALUES (@Tabela, @Akcija, @PrimarniKljuc, @NovaVrednost, @Korisnik)
END
GO

-- ================================================
-- DELJENI UTILITY: AŽURIRANJE ZALIHA
-- Objedinjuje sp_AzurirajZaliheNakonPrijeme i sp_SmanjiZalihe
-- @Smer: 1 = povećanje (prijema), -1 = smanjenje (izdavanje)
-- ================================================
CREATE PROCEDURE sp_AzurirajZalihe
    @ArtikalID INT,
    @LokacijaID INT,
    @Kolicina INT,
    @Smer INT,
    @Korisnik NVARCHAR(100)
AS
BEGIN
    BEGIN TRY
        DECLARE @IznosPromene INT = @Kolicina * @Smer

        IF EXISTS (SELECT 1 FROM Zalihe WHERE ArtikalID = @ArtikalID AND LokacijaID = @LokacijaID)
        BEGIN
            UPDATE Zalihe
            SET Kolicina = Kolicina + @IznosPromene,
                DisponibilnaKolicina = Kolicina + @IznosPromene - RezervovanoKolicina,
                DatumZadnjeIzmene = GETDATE()
            WHERE ArtikalID = @ArtikalID AND LokacijaID = @LokacijaID
        END
        ELSE IF @Smer = 1
        BEGIN
            INSERT INTO Zalihe (ArtikalID, LokacijaID, Kolicina, DisponibilnaKolicina)
            VALUES (@ArtikalID, @LokacijaID, @Kolicina, @Kolicina)
        END

        DECLARE @Opis NVARCHAR(MAX) = CASE @Smer
            WHEN 1 THEN 'Povećano za: '
            ELSE 'Smanjeno za: '
        END + CAST(@Kolicina AS NVARCHAR(100))

        EXEC sp_LogAudit 'Zalihe', 'UPDATE', @ArtikalID, @Opis, @Korisnik
    END TRY
    BEGIN CATCH
        DECLARE @Poruka NVARCHAR(200) = CASE @Smer
            WHEN 1 THEN 'Greška pri povećanju zalihe'
            ELSE 'Greška pri smanjenju zalihe'
        END
        RAISERROR(@Poruka, 16, 1)
    END CATCH
END
GO

-- ================================================
-- DELJENI VIEW: VREDNOST ZALIHE
-- Koristi se u sp_GetVrednostZalihe i sp_ABCAnaliza
-- ================================================
CREATE VIEW vw_VrednostZalihe
AS
    SELECT 
        a.ArtikalID,
        a.SifraArtikla,
        a.NazivArtikla,
        a.CenaKupovine,
        SUM(z.Kolicina) AS UkupnaKolicina,
        SUM(z.Kolicina) * a.CenaKupovine AS VrednostZalihe
    FROM Zalihe z
    INNER JOIN Artikli a ON z.ArtikalID = a.ArtikalID
    WHERE a.Aktivan = 1
    GROUP BY a.ArtikalID, a.SifraArtikla, a.NazivArtikla, a.CenaKupovine
GO

-- ================================================
-- 1. PROCEDURE ZA PRIJEMU MATERIJALA
-- ================================================
CREATE PROCEDURE sp_DodajPrijemu
    @BrojDokumenta NVARCHAR(50),
    @DobavljacID INT,
    @Napomena NVARCHAR(500) = NULL,
    @Korisnik NVARCHAR(100),
    @PrijemaID INT OUTPUT
AS
BEGIN
    BEGIN TRY
        INSERT INTO PrijemaMaterijala (BrojDokumenta, DobavljacID, Napomena, Status, Korisnik)
        VALUES (@BrojDokumenta, @DobavljacID, @Napomena, 'Otvorena', @Korisnik)
        
        SET @PrijemaID = SCOPE_IDENTITY()
        
        EXEC sp_LogAudit 'PrijemaMaterijala', 'INSERT', @PrijemaID, 
             @BrojDokumenta, @Korisnik
    END TRY
    BEGIN CATCH
        RAISERROR('Greška pri dodavanju prijeme', 16, 1)
    END CATCH
END
GO

-- ================================================
-- 2. PROCEDURE ZA DODAVANJE STAVKE PRIJEME
-- ================================================
CREATE PROCEDURE sp_DodajPrijemaStavku
    @PrijemaID INT,
    @ArtikalID INT,
    @KolicinaNarudjena INT,
    @CenaJedinice DECIMAL(18,2),
    @LotBroj NVARCHAR(50) = NULL,
    @DatumRoka DATE = NULL,
    @Korisnik NVARCHAR(100)
AS
BEGIN
    BEGIN TRY
        INSERT INTO PrijemaStavke (PrijemaID, ArtikalID, KolicinaNarudjena, CenaJedinice, LotBroj, DatumRoka)
        VALUES (@PrijemaID, @ArtikalID, @KolicinaNarudjena, @CenaJedinice, @LotBroj, @DatumRoka)
        
        DECLARE @Opis NVARCHAR(MAX) = 'Artikal ID: ' + CAST(@ArtikalID AS NVARCHAR(100))
        EXEC sp_LogAudit 'PrijemaStavke', 'INSERT', @PrijemaID, @Opis, @Korisnik
    END TRY
    BEGIN CATCH
        RAISERROR('Greška pri dodavanju stavke prijeme', 16, 1)
    END CATCH
END
GO

-- ================================================
-- 3. PROCEDURE ZA IZDAVANJE MATERIJALA
-- ================================================
CREATE PROCEDURE sp_DodajIzdavanje
    @BrojDokumenta NVARCHAR(50),
    @TipIzdavanja NVARCHAR(50),
    @Napomena NVARCHAR(500) = NULL,
    @Korisnik NVARCHAR(100),
    @IzdavanjeID INT OUTPUT
AS
BEGIN
    BEGIN TRY
        INSERT INTO IzdavanjeMaterijala (BrojDokumenta, TipIzdavanja, Napomena, Status, Korisnik)
        VALUES (@BrojDokumenta, @TipIzdavanja, @Napomena, 'Otvorena', @Korisnik)
        
        SET @IzdavanjeID = SCOPE_IDENTITY()
        
        EXEC sp_LogAudit 'IzdavanjeMaterijala', 'INSERT', @IzdavanjeID,
             @BrojDokumenta, @Korisnik
    END TRY
    BEGIN CATCH
        RAISERROR('Greška pri dodavanju izdavanja', 16, 1)
    END CATCH
END
GO

-- ================================================
-- 4. PROCEDURE ZA TRANSFER MATERIJALA
-- ================================================
CREATE PROCEDURE sp_DodajTransfer
    @BrojDokumenta NVARCHAR(50),
    @IzLokacijeID INT,
    @ULokacijuID INT,
    @Napomena NVARCHAR(500) = NULL,
    @Korisnik NVARCHAR(100),
    @TransferID INT OUTPUT
AS
BEGIN
    BEGIN TRY
        INSERT INTO Transferi (BrojDokumenta, IzLokacijeID, ULokacijuID, Napomena, Status, Korisnik)
        VALUES (@BrojDokumenta, @IzLokacijeID, @ULokacijuID, @Napomena, 'Otvorena', @Korisnik)
        
        SET @TransferID = SCOPE_IDENTITY()
        
        EXEC sp_LogAudit 'Transferi', 'INSERT', @TransferID,
             @BrojDokumenta, @Korisnik
    END TRY
    BEGIN CATCH
        RAISERROR('Greška pri dodavanju transfera', 16, 1)
    END CATCH
END
GO

-- ================================================
-- 5. PROCEDURE ZA INVENTURU
-- ================================================
CREATE PROCEDURE sp_DodajInventuru
    @BrojDokumenta NVARCHAR(50),
    @TipInventure NVARCHAR(20),
    @Napomena NVARCHAR(500) = NULL,
    @Korisnik NVARCHAR(100),
    @InventuraID INT OUTPUT
AS
BEGIN
    BEGIN TRY
        INSERT INTO Inventure (BrojDokumenta, TipInventure, Napomena, Status, Korisnik)
        VALUES (@BrojDokumenta, @TipInventure, @Napomena, 'U_toku', @Korisnik)
        
        SET @InventuraID = SCOPE_IDENTITY()
        
        EXEC sp_LogAudit 'Inventure', 'INSERT', @InventuraID,
             @BrojDokumenta, @Korisnik
    END TRY
    BEGIN CATCH
        RAISERROR('Greška pri dodavanju inventure', 16, 1)
    END CATCH
END
GO

-- ================================================
-- 6. PROCEDURE ZA PREGLED DOSTUPNE ZALIHE
-- ================================================
CREATE PROCEDURE sp_GetDostupnaZaliha
    @ArtikalID INT
AS
BEGIN
    SELECT 
        a.ArtikalID,
        a.SifraArtikla,
        a.NazivArtikla,
        SUM(z.DisponibilnaKolicina) AS UkupnaDostupna,
        SUM(z.Kolicina) AS UkupnaZaliha,
        SUM(z.RezervovanoKolicina) AS UkupnoRezervovano
    FROM Zalihe z
    INNER JOIN Artikli a ON z.ArtikalID = a.ArtikalID
    WHERE a.ArtikalID = @ArtikalID
    GROUP BY a.ArtikalID, a.SifraArtikla, a.NazivArtikla
END
GO

-- ================================================
-- 7. PROCEDURE ZA PREGLED ZALIHE PO LOKACIJAMA
-- ================================================
CREATE PROCEDURE sp_GetZalihaPoLokacijama
    @ArtikalID INT
AS
BEGIN
    SELECT 
        z.LokacijaID,
        sl.Opis AS NazivLokacije,
        z.Kolicina,
        z.RezervovanoKolicina,
        z.DisponibilnaKolicina
    FROM Zalihe z
    INNER JOIN SkladisneLokacije sl ON z.LokacijaID = sl.LokacijaID
    WHERE z.ArtikalID = @ArtikalID
    ORDER BY sl.ZonaID, sl.Red, sl.Polica
END
GO

-- ================================================
-- 8. PROCEDURE ZA PREGLED ARTIKALA SA NISKOM ZALIJHOM
-- ================================================
CREATE PROCEDURE sp_GetArtikliSaNiskomZalijhom
AS
BEGIN
    SELECT 
        a.ArtikalID,
        a.SifraArtikla,
        a.NazivArtikla,
        a.MinimalneStalje,
        SUM(z.Kolicina) AS UkupnaZaliha,
        a.MinimalneStalje - SUM(z.Kolicina) AS KolicinaZaNarudzbinu
    FROM Artikli a
    LEFT JOIN Zalihe z ON a.ArtikalID = z.ArtikalID
    WHERE a.Aktivan = 1
    GROUP BY a.ArtikalID, a.SifraArtikla, a.NazivArtikla, a.MinimalneStalje
    HAVING SUM(z.Kolicina) <= a.MinimalneStalje
    ORDER BY a.MinimalneStalje DESC
END
GO

-- ================================================
-- 9. PROCEDURE ZA PREGLED VREDNOSTI ZALIHE
-- Koristi deljeni vw_VrednostZalihe view
-- ================================================
CREATE PROCEDURE sp_GetVrednostZalihe
AS
BEGIN
    SELECT 
        SifraArtikla,
        NazivArtikla,
        CenaKupovine,
        UkupnaKolicina,
        VrednostZalihe
    FROM vw_VrednostZalihe
    ORDER BY VrednostZalihe DESC
END
GO

-- ================================================
-- 10. PROCEDURE ZA AUTENTIFIKACIJU KORISNIKA
-- ================================================
CREATE PROCEDURE sp_ValidacijaKorisnika
    @Korisnicko_Ime NVARCHAR(50),
    @Lozinka NVARCHAR(255),
    @KorisnikID INT OUTPUT,
    @Uloga NVARCHAR(50) OUTPUT
AS
BEGIN
    SELECT @KorisnikID = KorisnikID, @Uloga = Uloga
    FROM Korisnici
    WHERE Korisnicko_Ime = @Korisnicko_Ime 
    AND Lozinka = @Lozinka 
    AND Aktivan = 1
    
    IF @KorisnikID IS NULL
    BEGIN
        SET @KorisnikID = -1
        SET @Uloga = NULL
    END
END
GO

-- ================================================
-- 11. PROCEDURE ZA PREGLED ISTORIJE TRANSAKCIJA
-- ================================================
CREATE PROCEDURE sp_GetIstorijaTransakcija
    @DanaUnazad INT = 30
AS
BEGIN
    SELECT TOP 500
        AuditID,
        Tabela,
        Akcija,
        Korisnik,
        DatumAkcije,
        NovaVrednost
    FROM AuditLog
    WHERE DatumAkcije >= DATEADD(DAY, -@DanaUnazad, GETDATE())
    ORDER BY DatumAkcije DESC
END
GO

-- ================================================
-- 12. PROCEDURE ZA DOBIJANJE DOSTUPNIH LOKACIJA
-- ================================================
CREATE PROCEDURE sp_GetDostupneLokacije
AS
BEGIN
    SELECT 
        LokacijaID,
        ZonaID,
        Opis,
        JeLiSlobodna
    FROM SkladisneLokacije
    WHERE Aktivna = 1 AND JeLiSlobodna = 1
    ORDER BY ZonaID, Red, Polica
END
GO

-- ================================================
-- 13. PROCEDURE ZA ABC ANALIZU
-- Koristi deljeni vw_VrednostZalihe view
-- ================================================
CREATE PROCEDURE sp_ABCAnaliza
AS
BEGIN
    DECLARE @UkupnaVrednost DECIMAL(18,2)
    SELECT @UkupnaVrednost = SUM(VrednostZalihe) FROM vw_VrednostZalihe

    SELECT 
        ArtikalID,
        SifraArtikla,
        NazivArtikla,
        UkupnaKolicina AS Kolicina,
        CenaKupovine,
        VrednostZalihe AS Vrednost,
        CAST(ROUND((VrednostZalihe * 100 / @UkupnaVrednost), 2) AS DECIMAL(5,2)) AS ProcenatVrednosti,
        CASE 
            WHEN (VrednostZalihe * 100 / @UkupnaVrednost) >= 80 THEN 'A - Kritična'
            WHEN (VrednostZalihe * 100 / @UkupnaVrednost) >= 50 THEN 'B - Važna'
            ELSE 'C - Ostalo'
        END AS Kategorija
    FROM vw_VrednostZalihe
    ORDER BY VrednostZalihe DESC
END
GO

PRINT '================================'
PRINT 'Stored Procedures su uspešno kreirani!'
PRINT '================================'
