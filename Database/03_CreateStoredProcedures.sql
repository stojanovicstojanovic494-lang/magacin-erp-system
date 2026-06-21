-- ================================================
-- MAGACIN ERP SISTEM - STORED PROCEDURES
-- SQL Server 2022
-- ================================================

USE MagacinERP
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
        
        INSERT INTO AuditLog (Tabela, Akcija, PrimarniKljuc, NovaVrednost, Korisnik)
        VALUES ('PrijemaMaterijala', 'INSERT', CAST(@PrijemaID AS NVARCHAR(100)), 
                'Broj: ' + @BrojDokumenta, @Korisnik)
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
        
        INSERT INTO AuditLog (Tabela, Akcija, PrimarniKljuc, NovaVrednost, Korisnik)
        VALUES ('PrijemaStavke', 'INSERT', CAST(@PrijemaID AS NVARCHAR(100)), 
                'Artikal ID: ' + CAST(@ArtikalID AS NVARCHAR(100)), @Korisnik)
    END TRY
    BEGIN CATCH
        RAISERROR('Greška pri dodavanju stavke prijeme', 16, 1)
    END CATCH
END
GO

-- ================================================
-- 3. PROCEDURE ZA AŽURIRANJE ZALIHA NAKON PRIJEME
-- ================================================
CREATE PROCEDURE sp_AzurirajZaliheNakonPrijeme
    @ArtikalID INT,
    @LokacijaID INT,
    @Kolicina INT,
    @Korisnik NVARCHAR(100)
AS
BEGIN
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM Zalihe WHERE ArtikalID = @ArtikalID AND LokacijaID = @LokacijaID)
        BEGIN
            UPDATE Zalihe
            SET Kolicina = Kolicina + @Kolicina,
                DisponibilnaKolicina = Kolicina + @Kolicina - RezervovanoKolicina,
                DatumZadnjeIzmene = GETDATE()
            WHERE ArtikalID = @ArtikalID AND LokacijaID = @LokacijaID
        END
        ELSE
        BEGIN
            INSERT INTO Zalihe (ArtikalID, LokacijaID, Kolicina, DisponibilnaKolicina)
            VALUES (@ArtikalID, @LokacijaID, @Kolicina, @Kolicina)
        END
        
        INSERT INTO AuditLog (Tabela, Akcija, PrimarniKljuc, NovaVrednost, Korisnik)
        VALUES ('Zalihe', 'UPDATE', CAST(@ArtikalID AS NVARCHAR(100)), 
                'Kolicina: ' + CAST(@Kolicina AS NVARCHAR(100)), @Korisnik)
    END TRY
    BEGIN CATCH
        RAISERROR('Greška pri ažuriranju zalihe', 16, 1)
    END CATCH
END
GO

-- ================================================
-- 4. PROCEDURE ZA IZDAVANJE MATERIJALA
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
        
        INSERT INTO AuditLog (Tabela, Akcija, PrimarniKljuc, NovaVrednost, Korisnik)
        VALUES ('IzdavanjeMaterijala', 'INSERT', CAST(@IzdavanjeID AS NVARCHAR(100)), 
                'Broj: ' + @BrojDokumenta, @Korisnik)
    END TRY
    BEGIN CATCH
        RAISERROR('Greška pri dodavanju izdavanja', 16, 1)
    END CATCH
END
GO

-- ================================================
-- 5. PROCEDURE ZA SMANJENJE ZALIHA
-- ================================================
CREATE PROCEDURE sp_SmanjiZalihe
    @ArtikalID INT,
    @LokacijaID INT,
    @Kolicina INT,
    @Korisnik NVARCHAR(100)
AS
BEGIN
    BEGIN TRY
        UPDATE Zalihe
        SET Kolicina = Kolicina - @Kolicina,
            DisponibilnaKolicina = Kolicina - @Kolicina - RezervovanoKolicina,
            DatumZadnjeIzmene = GETDATE()
        WHERE ArtikalID = @ArtikalID AND LokacijaID = @LokacijaID
        
        INSERT INTO AuditLog (Tabela, Akcija, PrimarniKljuc, NovaVrednost, Korisnik)
        VALUES ('Zalihe', 'UPDATE', CAST(@ArtikalID AS NVARCHAR(100)), 
                'Smanjeno za: ' + CAST(@Kolicina AS NVARCHAR(100)), @Korisnik)
    END TRY
    BEGIN CATCH
        RAISERROR('Greška pri smanjenju zalihe', 16, 1)
    END CATCH
END
GO

-- ================================================
-- 6. PROCEDURE ZA TRANSFER MATERIJALA
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
        
        INSERT INTO AuditLog (Tabela, Akcija, PrimarniKljuc, NovaVrednost, Korisnik)
        VALUES ('Transferi', 'INSERT', CAST(@TransferID AS NVARCHAR(100)), 
                'Broj: ' + @BrojDokumenta, @Korisnik)
    END TRY
    BEGIN CATCH
        RAISERROR('Greška pri dodavanju transfera', 16, 1)
    END CATCH
END
GO

-- ================================================
-- 7. PROCEDURE ZA INVENTURU
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
        
        INSERT INTO AuditLog (Tabela, Akcija, PrimarniKljuc, NovaVrednost, Korisnik)
        VALUES ('Inventure', 'INSERT', CAST(@InventuraID AS NVARCHAR(100)), 
                'Broj: ' + @BrojDokumenta, @Korisnik)
    END TRY
    BEGIN CATCH
        RAISERROR('Greška pri dodavanju inventure', 16, 1)
    END CATCH
END
GO

-- ================================================
-- 8. PROCEDURE ZA PREGLED DOSTUPNE ZALIHE
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
-- 9. PROCEDURE ZA PREGLED ZALIHE PO LOKACIJAMA
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
-- 10. PROCEDURE ZA PREGLED ARTIKALA SA NISKOM ZALIJHOM
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
-- 11. PROCEDURE ZA PREGLED VREDNOSTI ZALIHE
-- ================================================
CREATE PROCEDURE sp_GetVrednostZalihe
AS
BEGIN
    SELECT 
        a.SifraArtikla,
        a.NazivArtikla,
        a.CenaKupovine,
        SUM(z.Kolicina) AS UkupnaKolicina,
        (SUM(z.Kolicina) * a.CenaKupovine) AS VrednostZalihe
    FROM Zalihe z
    INNER JOIN Artikli a ON z.ArtikalID = a.ArtikalID
    WHERE a.Aktivan = 1
    GROUP BY a.ArtikalID, a.SifraArtikla, a.NazivArtikla, a.CenaKupovine
    ORDER BY VrednostZalihe DESC
END
GO

-- ================================================
-- 12. PROCEDURE ZA AUTENTIFIKACIJU KORISNIKA
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
-- 13. PROCEDURE ZA PREGLED ISTORIJE TRANSAKCIJA
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
-- 14. PROCEDURE ZA DOBIJANJE DOSTUPNIH LOKACIJA
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
-- 15. PROCEDURE ZA ABC ANALIZU
-- ================================================
CREATE PROCEDURE sp_ABCAnaliza
AS
BEGIN
    DECLARE @UkupnaVrednost DECIMAL(18,2)
    
    SELECT @UkupnaVrednost = SUM(SUM(z.Kolicina) * a.CenaKupovine)
    FROM Zalihe z
    INNER JOIN Artikli a ON z.ArtikalID = a.ArtikalID
    WHERE a.Aktivan = 1
    
    SELECT 
        a.ArtikalID,
        a.SifraArtikla,
        a.NazivArtikla,
        SUM(z.Kolicina) AS Kolicina,
        a.CenaKupovine,
        (SUM(z.Kolicina) * a.CenaKupovine) AS Vrednost,
        CAST((SUM(z.Kolicina) * a.CenaKupovine) * 100 / @UkupnaVrednost AS DECIMAL(5,2)) AS ProcenatVrednosti,
        CASE 
            WHEN CAST((SUM(z.Kolicina) * a.CenaKupovine) * 100 / @UkupnaVrednost AS DECIMAL(5,2)) >= 80 THEN 'A - Kritična'
            WHEN CAST((SUM(z.Kolicina) * a.CenaKupovine) * 100 / @UkupnaVrednost AS DECIMAL(5,2)) >= 50 THEN 'B - Važna'
            ELSE 'C - Ostalo'
        END AS Kategorija
    FROM Zalihe z
    INNER JOIN Artikli a ON z.ArtikalID = a.ArtikalID
    WHERE a.Aktivan = 1
    GROUP BY a.ArtikalID, a.SifraArtikla, a.NazivArtikla, a.CenaKupovine
    ORDER BY Vrednost DESC
END
GO

PRINT '================================'
PRINT 'Stored Procedures su uspešno kreirani!'
PRINT '================================'
