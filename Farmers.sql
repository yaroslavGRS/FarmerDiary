-- Створення бази даних
CREATE DATABASE FarmerDiary;
USE FarmerDiary;

-- Створення таблиці User
CREATE TABLE User (
    UserID INT AUTO_INCREMENT PRIMARY KEY,
    Email VARCHAR(100) NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL
);

-- Створення таблиці Crop
CREATE TABLE Crop (
    CropID INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Area FLOAT NOT NULL,
    Quantity FLOAT NOT NULL,
    UserID INT,
    FOREIGN KEY (UserID) REFERENCES User(UserID) ON DELETE CASCADE
);

-- Створення таблиці Fertilizer
CREATE TABLE Fertilizer (
    FertilizerID INT AUTO_INCREMENT PRIMARY KEY,
    CropID INT,
    Type VARCHAR(100) NOT NULL,
    Amount FLOAT NOT NULL,
    FOREIGN KEY (CropID) REFERENCES Crop(CropID) ON DELETE CASCADE
);

-- Створення таблиці Event
CREATE TABLE Event (
    EventID INT AUTO_INCREMENT PRIMARY KEY,
    EventName VARCHAR(100) NOT NULL,
    EventDate DATE NOT NULL,
    CropID INT,
    FOREIGN KEY (CropID) REFERENCES Crop(CropID) ON DELETE CASCADE
);
