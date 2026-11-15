-- Gör så att Å Ä Ö fungerar. 
SET NAMES utf8mb4;

DROP DATABASE IF EXISTS holidaymaker;
CREATE DATABASE holidaymaker CHARACTER SET utf8mb4;
USE holidaymaker;


CREATE TABLE Ort (
    OrtId  INT          PRIMARY KEY AUTO_INCREMENT,
    Namn   VARCHAR(60)  NOT NULL UNIQUE
);


CREATE TABLE Boende (
    BoendeId             INT           PRIMARY KEY AUTO_INCREMENT,
    Namn                 VARCHAR(100)  NOT NULL,
    OrtId                INT           NOT NULL,
    HarPool              BOOLEAN       NOT NULL DEFAULT FALSE,
    HarRestaurang        BOOLEAN       NOT NULL DEFAULT FALSE,
    AvstandStrandMeter   INT           NOT NULL CHECK (AvstandStrandMeter >= 0),
    AvstandCentrumMeter  INT           NOT NULL CHECK (AvstandCentrumMeter >= 0),
    Omdome               TINYINT       NOT NULL CHECK (Omdome BETWEEN 1 AND 5),

    FOREIGN KEY (OrtId) REFERENCES Ort (OrtId),
    UNIQUE (Namn, OrtId)   
);


CREATE TABLE Rum (
    RumId        INT           PRIMARY KEY AUTO_INCREMENT,
    BoendeId     INT           NOT NULL,
    Rumsnummer   VARCHAR(10)   NOT NULL,
    MaxPersoner  TINYINT       NOT NULL CHECK (MaxPersoner > 0),
    PrisPerNatt  DECIMAL(8,2)  NOT NULL CHECK (PrisPerNatt >= 0),

    FOREIGN KEY (BoendeId) REFERENCES Boende (BoendeId),
    UNIQUE (BoendeId, Rumsnummer)
);


CREATE TABLE Kund (
    KundId     INT           PRIMARY KEY AUTO_INCREMENT,
    Fornamn    VARCHAR(50)   NOT NULL,
    Efternamn  VARCHAR(50)   NOT NULL,
    Epost      VARCHAR(100)  NOT NULL UNIQUE,
    Telefon    VARCHAR(20)   NOT NULL
);


CREATE TABLE Bokning (
    BokningId      INT       PRIMARY KEY AUTO_INCREMENT,
    KundId         INT       NOT NULL,
    RumId          INT       NOT NULL,
    Incheckning    DATE      NOT NULL,
    Utcheckning    DATE      NOT NULL,
    AntalPersoner  TINYINT   NOT NULL CHECK (AntalPersoner > 0),
    Skapad         DATETIME  NOT NULL DEFAULT CURRENT_TIMESTAMP,

    FOREIGN KEY (KundId) REFERENCES Kund (KundId),
    FOREIGN KEY (RumId)  REFERENCES Rum (RumId),
    CHECK (Utcheckning > Incheckning)   
);

CREATE INDEX IX_Bokning_Rum_Datum ON Bokning (RumId, Incheckning, Utcheckning);