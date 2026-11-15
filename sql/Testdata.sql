-- OBS AI genererad testdata (vilket var tillåtet)
SET NAMES utf8mb4;

USE holidaymaker;

INSERT INTO Ort (OrtId, Namn) VALUES
    (1, 'Båstad'),
    (2, 'Varberg'),
    (3, 'Ystad');

INSERT INTO Boende (BoendeId, Namn, OrtId, HarPool, HarRestaurang,
                    AvstandStrandMeter, AvstandCentrumMeter, Omdome) VALUES
    (1, 'Hotell Skansen',        1, TRUE,  TRUE,   150,  400, 5),
    (2, 'Pensionat Solgården',   1, FALSE, FALSE,  900,  200, 3),
    (3, 'Båstad Strandhotell',   1, TRUE,  FALSE,   50, 1500, 4),
    (4, 'Varbergs Kusthotell',   2, FALSE, TRUE,   300,  800, 4),
    (5, 'Hotell Fästningen',     2, TRUE,  TRUE,  1200,  100, 2),
    (6, 'Ystad Gästgiveri',      3, FALSE, TRUE,   600,  300, 3);

INSERT INTO Rum (RumId, BoendeId, Rumsnummer, MaxPersoner, PrisPerNatt) VALUES
    -- Hotell Skansen
    ( 1, 1, '101', 1,  895.00),
    ( 2, 1, '102', 2, 1290.00),
    ( 3, 1, '103', 2, 1350.00),
    ( 4, 1, '104', 4, 2190.00),
    ( 5, 1, '201', 4, 2390.00),
    -- Pensionat Solgården
    ( 6, 2, '1',   1,  550.00),
    ( 7, 2, '2',   2,  790.00),
    ( 8, 2, '3',   2,  790.00),
    ( 9, 2, '4',   4, 1250.00),
    -- Båstad Strandhotell
    (10, 3, 'S1',  2, 1490.00),
    (11, 3, 'S2',  2, 1590.00),
    (12, 3, 'S3',  4, 2690.00),
    (13, 3, 'S4',  1,  990.00),
    -- Varbergs Kusthotell
    (14, 4, '11',  1,  750.00),
    (15, 4, '12',  2, 1090.00),
    (16, 4, '13',  3, 1490.00),
    (17, 4, '14',  4, 1790.00),
    -- Hotell Fästningen
    (18, 5, 'A',   2,  980.00),
    (19, 5, 'B',   2, 1020.00),
    (20, 5, 'C',   4, 1650.00),
    -- Ystad Gästgiveri
    (21, 6, 'G1',  1,  650.00),
    (22, 6, 'G2',  2,  890.00);

INSERT INTO Kund (KundId, Fornamn, Efternamn, Epost, Telefon) VALUES
    (1, 'Lina',   'Bergström', 'lina.bergstrom@exempel.se', '0701234501'),
    (2, 'Omar',   'Haddad',    'omar.haddad@exempel.se',    '0701234502'),
    (3, 'Sara',   'Nilsson',   'sara.nilsson@exempel.se',   '0701234503'),
    (4, 'Johan',  'Lindqvist', 'johan.lindqvist@exempel.se','0701234504'),
    (5, 'Elif',   'Kaya',      'elif.kaya@exempel.se',      '0701234505'),
    (6, 'Mikael', 'Ek',        'mikael.ek@exempel.se',      '0701234506');

INSERT INTO Bokning (BokningId, KundId, RumId, Incheckning, Utcheckning, AntalPersoner) VALUES
    ( 1, 1,  2, '2026-12-10', '2026-12-13', 2),  
    ( 2, 2,  1, '2026-12-01', '2026-12-05', 1),
    ( 3, 3,  4, '2026-12-05', '2026-12-12', 3),
    ( 4, 1,  4, '2026-12-20', '2026-12-27', 4),
    ( 5, 4,  3, '2026-12-15', '2026-12-18', 2),
    ( 6, 5,  5, '2026-12-24', '2026-12-31', 4),
    ( 7, 2,  6, '2026-12-02', '2026-12-04', 1),
    ( 8, 3,  7, '2026-12-08', '2026-12-15', 2),
    ( 9, 6,  8, '2026-12-10', '2026-12-11', 1),
    (10, 4,  9, '2026-12-18', '2026-12-22', 4),
    (11, 5, 10, '2026-12-03', '2026-12-10', 2),  
    (12, 1, 11, '2026-12-12', '2026-12-19', 2),
    (13, 6, 12, '2026-12-22', '2026-12-29', 3),
    (14, 2, 13, '2026-12-06', '2026-12-08', 1),
    (15, 3, 14, '2026-12-01', '2026-12-03', 1),
    (16, 4, 15, '2026-12-14', '2026-12-21', 2),
    (17, 5, 16, '2026-12-05', '2026-12-09', 3),
    (18, 6, 17, '2026-12-23', '2026-12-30', 4),
    (19, 1, 18, '2026-12-10', '2026-12-17', 2),
    (20, 2, 19, '2026-12-19', '2026-12-26', 1),
    (21, 3, 20, '2026-12-01', '2026-12-08', 4),
    (22, 4, 21, '2026-12-20', '2026-12-27', 1),  
    (23, 5, 22, '2026-12-20', '2026-12-27', 2), 
    (24, 6,  1, '2026-12-15', '2026-12-16', 1),
    (25, 2,  2, '2026-12-20', '2026-12-24', 2),
    (26, 3,  5, '2026-12-10', '2026-12-14', 3),
    (27, 1, 16, '2026-12-26', '2026-12-31', 2);
