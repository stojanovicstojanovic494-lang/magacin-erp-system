-- ================================================
-- MAGACIN ERP SISTEM - TEST PODACI
-- SQL Server 2022
-- ================================================

USE MagacinERP
GO

-- ================================================
-- 1. JEDINICE MERE
-- ================================================
INSERT INTO JediniceMere (Naziv, Skracenica, Opis) VALUES
('Komad', 'kom', 'Jedinična mera - komad'),
('Kilogram', 'kg', 'Težinska mera - kilogram'),
('Litar', 'l', 'Zapreminska mera - litar'),
('Metar', 'm', 'Dužinska mera - metar'),
('Pakovanje', 'pak', 'Pakovanje'),
('Paletna', 'pal', 'Paletna količina'),
('Kutija', 'kut', 'Kutija')
GO

-- ================================================
-- 2. KATEGORIJE ARTIKALA
-- ================================================
INSERT INTO KategorijeArtikala (Naziv, Opis) VALUES
('Elektrotehnika', 'Električni elementi i komponente'),
('Mehanika', 'Mehanički delovi i sklopovi'),
('Hemikalije', 'Hemijske supstance i materijali'),
('Tekstil', 'Tekstilni proizvodi'),
('Metali', 'Metalni delovi i materijali'),
('Plastika', 'Plastični proizvodi'),
('Ambalaža', 'Ambalaža i pakovanje'),
('Razno', 'Ostali proizvodi')
GO

-- ================================================
-- 3. DOBAVLJACI
-- ================================================
INSERT INTO Dobavljaci (NazivFirme, KontaktOsoba, Email, Telefon, Adresa, Grad, PostanskiBroj, Drzava, PIB, Matični) VALUES
('ELEKTRO d.o.o.', 'Marko Marković', 'marko@elektro.rs', '+381 11 123 4567', 'Bulevar oslobođenja 50', 'Beograd', '11000', 'Srbija', '123456789', '12345678'),
('METAL Imports', 'Jovana Jovanović', 'jovana@metal.rs', '+381 21 987 6543', 'Ulica Francuska 15', 'Novi Sad', '21000', 'Srbija', '987654321', '98765432'),
('HEMIJA i hemikalije', 'Petar Petrović', 'petar@hemija.rs', '+381 12 555 1111', 'Industrijska zona 5', 'Smederevo', '11300', 'Srbija', '555666777', '55566677'),
('PLASTIK pro', 'Ana Anić', 'ana@plastik.rs', '+381 13 222 3333', 'Put Maršala Tita 100', 'Nis', '18000', 'Srbija', '111222333', '11122233')
GO

-- ================================================
-- 4. ZONE U MAGACINU
-- ================================================
INSERT INTO Zone (Naziv, Opis, Kapacitet) VALUES
('ZONA A - Elektronika', 'Zona za električne komponente', 500),
('ZONA B - Metali', 'Zona za metalne delove', 300),
('ZONA C - Hemikalije', 'Zona za hemijske materijale', 200),
('ZONA D - Razno', 'Zona za ostale proizvode', 400)
GO

-- ================================================
-- 5. SKLADISNE LOKACIJE
-- ================================================
INSERT INTO SkladisneLokacije (ZonaID, Red, Polica, Opis) VALUES
-- ZONA A - Elektronika
(1, 1, 1, 'Polica A1-1'),
(1, 1, 2, 'Polica A1-2'),
(1, 2, 1, 'Polica A2-1'),
(1, 2, 2, 'Polica A2-2'),
-- ZONA B - Metali
(2, 1, 1, 'Polica B1-1'),
(2, 1, 2, 'Polica B1-2'),
-- ZONA C - Hemikalije
(3, 1, 1, 'Polica C1-1'),
-- ZONA D - Razno
(4, 1, 1, 'Polica D1-1'),
(4, 1, 2, 'Polica D1-2')
GO

-- ================================================
-- 6. ARTIKLI
-- ================================================
INSERT INTO Artikli (SifraArtikla, NazivArtikla, KategorijaID, JedinicaMereID, CenaKupovine, CenaProdaje, MinimalneStalje, MaksimalneStalje, Tezina, Zapremina, Opis, Barkod) VALUES
('EL001', 'Kondenzator 100μF', 1, 1, 50.00, 75.00, 20, 200, 0.010, 0.001, 'Elektrolitski kondenzator', '5901234123457'),
('EL002', 'Otpornik 1kΩ', 1, 1, 10.00, 15.00, 100, 500, 0.001, 0.0001, 'Film otpornik', '5901234123458'),
('ME001', 'Vijak M8x20', 2, 1, 2.50, 4.00, 200, 1000, 0.020, 0.005, 'Metalni vijak', '5901234123459'),
('ME002', 'Matica M8', 2, 1, 1.50, 2.50, 300, 1500, 0.010, 0.003, 'Metalna matica', '5901234123460'),
('HE001', 'Ulje motorno 10W40', 3, 3, 800.00, 1200.00, 50, 200, 0.900, 1.000, 'Motorno ulje Premium', '5901234123461'),
('HE002', 'Rastvarač aceton', 3, 3, 300.00, 450.00, 30, 100, 0.790, 1.000, 'Čist aceton', '5901234123462'),
('PL001', 'Plastična kutija 30x20x15', 6, 1, 45.00, 65.00, 50, 300, 0.150, 0.009, 'Kutija za pakovanje', '5901234123463'),
('TE001', 'Pamučna tkanina 1m', 4, 4, 200.00, 350.00, 20, 100, 0.100, 0.001, 'Prirodna pamučna tkanina', '5901234123464'),
('AM001', 'Kartonska kutija A4', 7, 1, 20.00, 30.00, 500, 2000, 0.050, 0.003, 'Kutija za dokumente', '5901234123465'),
('RZ001', 'Žica za vezivanje 1kg', 5, 2, 150.00, 200.00, 10, 50, 1.000, 0.001, 'Čelična žica', '5901234123466')
GO

-- ================================================
-- 7. ZALIHE (INICIJALNE KOLICINE)
-- ================================================
INSERT INTO Zalihe (ArtikalID, LokacijaID, Kolicina, RezervovanoKolicina, DisponibilnaKolicina) VALUES
(1, 1, 100, 0, 100),
(2, 2, 250, 50, 200),
(3, 5, 500, 100, 400),
(4, 5, 800, 200, 600),
(5, 7, 75, 0, 75),
(6, 7, 40, 10, 30),
(7, 8, 150, 0, 150),
(8, 9, 50, 0, 50),
(9, 8, 300, 50, 250),
(10, 5, 25, 5, 20)
GO

-- ================================================
-- 8. LOT / SERIJSKI BROJEVI
-- ================================================
INSERT INTO LotiBrojevi (ArtikalID, LotBroj, SerijskiBroj, DatumProizvodnje, DatumRoka, Kolicina, Status) VALUES
(1, 'LOT2024001', 'SER0001', '2024-01-15', '2025-01-15', 50, 'Dostupan'),
(1, 'LOT2024002', 'SER0002', '2024-02-10', '2025-02-10', 50, 'Dostupan'),
(2, 'LOT2024003', 'SER0003', '2024-01-20', '2025-01-20', 200, 'Dostupan'),
(5, 'LOT2024004', 'SER0004', '2024-03-01', '2025-03-01', 75, 'Dostupan'),
(6, 'LOT2024005', 'SER0005', '2024-02-15', '2025-02-15', 40, 'Dostupan')
GO

-- ================================================
-- 9. TIPOVI DOKUMENATA
-- ================================================
INSERT INTO TipoviDokumenata (Naziv, Skracenica, Opis) VALUES
('GRN - Prijema Robe', 'GRN', 'Goods Receipt Note'),
('Izlazni Nalog', 'IN', 'Nalog za izdavanje'),
('Transfer', 'TR', 'Interni transfer'),
('Inventura', 'INV', 'Prebrojavnaje zalihe'),
('Korekcija', 'KOR', 'Korekcija zalihe')
GO

-- ================================================
-- 10. KORISNICI
-- ================================================
-- NOTE: In production, generate unique salts via CRYPT_GEN_RANDOM(32) per user.
-- These test salts are fixed so the seed script is deterministic.
DECLARE @Salt1 VARBINARY(32) = 0x0102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F20
DECLARE @Salt2 VARBINARY(32) = 0x2122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F40
DECLARE @Salt3 VARBINARY(32) = 0x4142434445464748494A4B4C4D4E4F505152535455565758595A5B5C5D5E5F60
DECLARE @Salt4 VARBINARY(32) = 0x6162636465666768696A6B6C6D6E6F707172737475767778797A7B7C7D7E7F80

INSERT INTO Korisnici (Korisnicko_Ime, LozinkaHash, LozinkaSalt, ImeKorisnika, Prezime, Email, Uloga, Aktivan) VALUES
('admin',     dbo.fn_HashPassword('admin123', @Salt1),     @Salt1, 'Administratski', 'Korisnik', 'admin@magacin.rs', 'Admin', 1),
('magaciner', dbo.fn_HashPassword('magacin123', @Salt2),   @Salt2, 'Marko', 'Marković', 'marko@magacin.rs', 'Magaciner', 1),
('magaciner2',dbo.fn_HashPassword('magacin123', @Salt3),   @Salt3, 'Jovana', 'Jovanović', 'jovana@magacin.rs', 'Magaciner', 1),
('pregled',   dbo.fn_HashPassword('pregled123', @Salt4),   @Salt4, 'Pregleda', 'Korisnik', 'pregled@magacin.rs', 'Pregled', 1)
GO

-- ================================================
-- 11. DOZVOLE KORISNIKA
-- ================================================
INSERT INTO Dozvole (KorisnikID, Opis, Aktivan) VALUES
(1, 'Pristup svim modulima', 1),
(1, 'Upravljanje korisnicima', 1),
(1, 'Upravljanje parametrima', 1),
(2, 'Prijema materijala', 1),
(2, 'Izdavanje materijala', 1),
(2, 'Prebrojavanje zalihe', 1),
(3, 'Prijema materijala', 1),
(3, 'Izdavanje materijala', 1),
(4, 'Pregled izveštaja', 1)
GO

-- ================================================
-- 12. PARAMETRI SISTEMA
-- ================================================
INSERT INTO ParametriSistema (NazivParametra, Vrednost, TipParametra, Opis) VALUES
('Naziv_Preduzeća', 'MAGACIN d.o.o.', 'String', 'Naziv preduzeća'),
('Email_Preduzeća', 'info@magacin.rs', 'String', 'Email preduzeća'),
('Telefon_Preduzeća', '+381 11 123 4567', 'String', 'Telefonski broj'),
('Minimalna_Stalja_Procenat', '10', 'Decimal', 'Procenat minimalne stalје'),
('Maksimalna_Stalja_Procenat', '100', 'Decimal', 'Procenat maksimalne stalје'),
('Slanje_Email_Obaveštenja', 'true', 'Bool', 'Slanje email obaveštenja'),
('Valuta', 'RSD', 'String', 'Valuta sistema'),
('Jezik', 'Srpski', 'String', 'Jezik sistema')
GO

PRINT '================================'
PRINT 'Test podaci su uspešno učitani!'
PRINT '================================'
PRINT ''
PRINT 'Test korisnici su kreirani (videti dokumentaciju za lozinke).'
