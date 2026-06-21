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
    SET NOCOUNT ON
    BEGIN TRY
        BEGIN TRANSACTION

        INSERT INTO PrijemaMaterijala (BrojDokumenta, DobavljacID, Napomena, Status, Korisnik)
        VALUES (@BrojDokumenta, @DobavljacID, @Napomena, 'Otvorena', @Korisnik)
        
        SET @PrijemaID = SCOPE_IDENTITY()
        
        INSERT INTO AuditLog (Tabela, Akcija, PrimarniKljuc, NovaVrednost, Korisnik)
        VALUES ('PrijemaMaterijala', 'INSERT', CAST(@PrijemaID AS NVARCHAR(100)), 
                'Broj: ' + @BrojDokumenta, @Korisnik)

        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        DECLARE @ErrMsg1 NVARCHAR(4000) = ERROR_MESSAGE()
        DECLARE @ErrSev1 INT = ERROR_SEVERITY()
        DECLARE @ErrState1 INT = ERROR_STATE()
        RAISERROR(@ErrMsg1, @ErrSev1, @ErrState1)
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
    SET NOCOUNT ON

    IF @KolicinaNarudjena <= 0
    BEGIN
        RAISERROR('Kolicina mora biti pozitivan broj', 16, 1)
        RETURN
    END

    BEGIN TRY
        BEGIN TRANSACTION

        INSERT INTO PrijemaStavke (PrijemaID, ArtikalID, KolicinaNarudjena, CenaJedinice, LotBroj, DatumRoka)
        VALUES (@PrijemaID, @ArtikalID, @KolicinaNarudjena, @CenaJedinice, @LotBroj, @DatumRoka)
        
        INSERT INTO AuditLog (Tabela, Akcija, PrimarniKljuc, NovaVrednost, Korisnik)
        VALUES ('PrijemaStavke', 'INSERT', CAST(@PrijemaID AS NVARCHAR(100)), 
                'Artikal ID: ' + CAST(@ArtikalID AS NVARCHAR(100)), @Korisnik)

        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        DECLARE @ErrMsg2 NVARCHAR(4000) = ERROR_MESSAGE()
        DECLARE @ErrSev2 INT = ERROR_SEVERITY()
        DECLARE @ErrState2 INT = ERROR_STATE()
        RAISERROR(@ErrMsg2, @ErrSev2, @ErrState2)
    END CATCH
END
GO

-- ================================================
-- 3. PROCEDURE ZA AZURIRANJE ZALIHA NAKON PRIJEME
-- ================================================
CREATE PROCEDURE sp_AzurirajZaliheNakonPrijeme
    @ArtikalID INT,
    @LokacijaID INT,
    @Kolicina INT,
    @Korisnik NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON

    IF @Kolicina <= 0
    BEGIN
        RAISERROR('Kolicina mora biti pozitivan broj', 16, 1)
        RETURN
    END

    BEGIN TRY
        BEGIN TRANSACTION

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

        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        DECLARE @ErrMsg3 NVARCHAR(4000) = ERROR_MESSAGE()
        DECLARE @ErrSev3 INT = ERROR_SEVERITY()
        DECLARE @ErrState3 INT = ERROR_STATE()
        RAISERROR(@ErrMsg3, @ErrSev3, @ErrState3)
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
    SET NOCOUNT ON
    BEGIN TRY
        BEGIN TRANSACTION

        INSERT INTO IzdavanjeMaterijala (BrojDokumenta, TipIzdavanja, Napomena, Status, Korisnik)
        VALUES (@BrojDokumenta, @TipIzdavanja, @Napomena, 'Otvorena', @Korisnik)
        
        SET @IzdavanjeID = SCOPE_IDENTITY()
        
        INSERT INTO AuditLog (Tabela, Akcija, PrimarniKljuc, NovaVrednost, Korisnik)
        VALUES ('IzdavanjeMaterijala', 'INSERT', CAST(@IzdavanjeID AS NVARCHAR(100)), 
                'Broj: ' + @BrojDokumenta, @Korisnik)

        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        DECLARE @ErrMsg4 NVARCHAR(4000) = ERROR_MESSAGE()
        DECLARE @ErrSev4 INT = ERROR_SEVERITY()
        DECLARE @ErrState4 INT = ERROR_STATE()
        RAISERROR(@ErrMsg4, @ErrSev4, @ErrState4)
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
    SET NOCOUNT ON

    IF @Kolicina <= 0
    BEGIN
        RAISERROR('Kolicina mora biti pozitivan broj', 16, 1)
        RETURN
    END

    DECLARE @TrenutnaKolicina INT
    SELECT @TrenutnaKolicina = Kolicina
    FROM Zalihe
    WHERE ArtikalID = @ArtikalID AND LokacijaID = @LokacijaID

    IF @TrenutnaKolicina IS NULL
    BEGIN
        RAISERROR('Zaliha ne postoji za dati artikal i lokaciju', 16, 1)
        RETURN
    END

    IF @TrenutnaKolicina < @Kolicina
    BEGIN
        RAISERROR('Nedovoljna kolicina na zalihama', 16, 1)
        RETURN
    END

    BEGIN TRY
        BEGIN TRANSACTION

        UPDATE Zalihe
        SET Kolicina = Kolicina - @Kolicina,
            DisponibilnaKolicina = Kolicina - @Kolicina - RezervovanoKolicina,
            DatumZadnjeIzmene = GETDATE()
        WHERE ArtikalID = @ArtikalID AND LokacijaID = @LokacijaID
        
        INSERT INTO AuditLog (Tabela, Akcija, PrimarniKljuc, NovaVrednost, Korisnik)
        VALUES ('Zalihe', 'UPDATE', CAST(@ArtikalID AS NVARCHAR(100)), 
                'Smanjeno za: ' + CAST(@Kolicina AS NVARCHAR(100)), @Korisnik)

        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        DECLARE @ErrMsg5 NVARCHAR(4000) = ERROR_MESSAGE()
        DECLARE @ErrSev5 INT = ERROR_SEVERITY()
        DECLARE @ErrState5 INT = ERROR_STATE()
        RAISERROR(@ErrMsg5, @ErrSev5, @ErrState5)
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
    SET NOCOUNT ON

    IF @IzLokacijeID = @ULokacijuID
    BEGIN
        RAISERROR('Izvorna i odredisna lokacija ne mogu biti iste', 16, 1)
        RETURN
    END

    BEGIN TRY
        BEGIN TRANSACTION

        INSERT INTO Transferi (BrojDokumenta, IzLokacijeID, ULokacijuID, Napomena, Status, Korisnik)
        VALUES (@BrojDokumenta, @IzLokacijeID, @ULokacijuID, @Napomena, 'Otvorena', @Korisnik)
        
        SET @TransferID = SCOPE_IDENTITY()
        
        INSERT INTO AuditLog (Tabela, Akcija, PrimarniKljuc, NovaVrednost, Korisnik)
        VALUES ('Transferi', 'INSERT', CAST(@TransferID AS NVARCHAR(100)), 
                'Broj: ' + @BrojDokumenta, @Korisnik)

        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        DECLARE @ErrMsg6 NVARCHAR(4000) = ERROR_MESSAGE()
        DECLARE @ErrSev6 INT = ERROR_SEVERITY()
        DECLARE @ErrState6 INT = ERROR_STATE()
        RAISERROR(@ErrMsg6, @ErrSev6, @ErrState6)
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
    SET NOCOUNT ON
    BEGIN TRY
        BEGIN TRANSACTION

        INSERT INTO Inventure (BrojDokumenta, TipInventure, Napomena, Status, Korisnik)
        VALUES (@BrojDokumenta, @TipInventure, @Napomena, 'U_toku', @Korisnik)
        
        SET @InventuraID = SCOPE_IDENTITY()
        
        INSERT INTO AuditLog (Tabela, Akcija, PrimarniKljuc, NovaVrednost, Korisnik)
        VALUES ('Inventure', 'INSERT', CAST(@InventuraID AS NVARCHAR(100)), 
                'Broj: ' + @BrojDokumenta, @Korisnik)

        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        DECLARE @ErrMsg7 NVARCHAR(4000) = ERROR_MESSAGE()
        DECLARE @ErrSev7 INT = ERROR_SEVERITY()
        DECLARE @ErrState7 INT = ERROR_STATE()
        RAISERROR(@ErrMsg7, @ErrSev7, @ErrState7)
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
    SET NOCOUNT ON
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
    SET NOCOUNT ON
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
    SET NOCOUNT ON
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
    SET NOCOUNT ON
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
-- Uses salted SHA-256 hash comparison with brute-force lockout.
-- ================================================
CREATE PROCEDURE sp_ValidacijaKorisnika
    @Korisnicko_Ime NVARCHAR(50),
    @Lozinka NVARCHAR(255),
    @KorisnikID INT OUTPUT,
    @Uloga NVARCHAR(50) OUTPUT
AS
BEGIN
    SET NOCOUNT ON
    SET @KorisnikID = -1
    SET @Uloga = NULL

    DECLARE @StoredHash VARBINARY(64)
    DECLARE @StoredSalt VARBINARY(32)
    DECLARE @NeuspesniPokusaji INT
    DECLARE @ZakljucanDo DATETIME
    DECLARE @TempKorisnikID INT

    SELECT
        @TempKorisnikID = KorisnikID,
        @StoredHash = LozinkaHash,
        @StoredSalt = LozinkaSalt,
        @NeuspesniPokusaji = NeuspesniPokusaji,
        @ZakljucanDo = ZakljucanDo
    FROM Korisnici
    WHERE Korisnicko_Ime = @Korisnicko_Ime AND Aktivan = 1

    IF @TempKorisnikID IS NULL
        RETURN

    IF @ZakljucanDo IS NOT NULL AND @ZakljucanDo > GETDATE()
    BEGIN
        RAISERROR('Nalog je privremeno zakljucan. Pokusajte ponovo kasnije.', 16, 1)
        RETURN
    END

    DECLARE @InputHash VARBINARY(64) = dbo.fn_HashPassword(@Lozinka, @StoredSalt)

    IF @InputHash = @StoredHash
    BEGIN
        SET @KorisnikID = @TempKorisnikID
        SELECT @Uloga = Uloga FROM Korisnici WHERE KorisnikID = @TempKorisnikID

        UPDATE Korisnici
        SET NeuspesniPokusaji = 0, ZakljucanDo = NULL
        WHERE KorisnikID = @TempKorisnikID

        INSERT INTO AuditLog (Tabela, Akcija, PrimarniKljuc, NovaVrednost, Korisnik)
        VALUES ('Korisnici', 'LOGIN', CAST(@TempKorisnikID AS NVARCHAR(100)),
                'Uspesna prijava', @Korisnicko_Ime)
    END
    ELSE
    BEGIN
        SET @NeuspesniPokusaji = ISNULL(@NeuspesniPokusaji, 0) + 1

        UPDATE Korisnici
        SET NeuspesniPokusaji = @NeuspesniPokusaji,
            ZakljucanDo = CASE WHEN @NeuspesniPokusaji >= 5
                               THEN DATEADD(MINUTE, 15, GETDATE())
                               ELSE ZakljucanDo END
        WHERE KorisnikID = @TempKorisnikID

        INSERT INTO AuditLog (Tabela, Akcija, PrimarniKljuc, NovaVrednost, Korisnik)
        VALUES ('Korisnici', 'LOGIN_FAIL', CAST(@TempKorisnikID AS NVARCHAR(100)),
                'Neuspesna prijava (pokusaj ' + CAST(@NeuspesniPokusaji AS NVARCHAR(10)) + ')',
                @Korisnicko_Ime)
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
    SET NOCOUNT ON

    IF @DanaUnazad <= 0 OR @DanaUnazad > 365
        SET @DanaUnazad = 30

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
    SET NOCOUNT ON
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
    SET NOCOUNT ON

    DECLARE @UkupnaVrednost DECIMAL(18,2)

    SELECT @UkupnaVrednost = SUM(sub.Vrednost)
    FROM (
        SELECT SUM(z.Kolicina) * a.CenaKupovine AS Vrednost
        FROM Zalihe z
        INNER JOIN Artikli a ON z.ArtikalID = a.ArtikalID
        WHERE a.Aktivan = 1
        GROUP BY a.ArtikalID, a.CenaKupovine
    ) sub

    IF @UkupnaVrednost IS NULL OR @UkupnaVrednost = 0
    BEGIN
        RAISERROR('Nema podataka za ABC analizu', 16, 1)
        RETURN
    END

    SELECT 
        a.ArtikalID,
        a.SifraArtikla,
        a.NazivArtikla,
        SUM(z.Kolicina) AS Kolicina,
        a.CenaKupovine,
        (SUM(z.Kolicina) * a.CenaKupovine) AS Vrednost,
        CAST((SUM(z.Kolicina) * a.CenaKupovine) * 100 / @UkupnaVrednost AS DECIMAL(5,2)) AS ProcenatVrednosti,
        CASE 
            WHEN CAST((SUM(z.Kolicina) * a.CenaKupovine) * 100 / @UkupnaVrednost AS DECIMAL(5,2)) >= 80 THEN 'A - Kriticna'
            WHEN CAST((SUM(z.Kolicina) * a.CenaKupovine) * 100 / @UkupnaVrednost AS DECIMAL(5,2)) >= 50 THEN 'B - Vazna'
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
PRINT 'Stored Procedures su uspesno kreirani!'
PRINT '================================'
