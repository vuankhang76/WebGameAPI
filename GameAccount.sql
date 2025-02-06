-- Tạo database
CREATE DATABASE GameAccountStore
GO

USE GameAccountStore
GO

-- Tạo bảng Categories
CREATE TABLE Categories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
)
GO

-- Tạo bảng Users
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL DEFAULT 'User', -- 'Admin' hoặc 'User'
    Balance DECIMAL(18,2) NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
)
GO

-- Tạo bảng GameAccounts
CREATE TABLE GameAccounts (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CategoryId INT NOT NULL,
    Title NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX),
    GameType NVARCHAR(50) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Available', -- 'Available', 'Sold', 'Pending'
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
)
GO

-- Tạo bảng GameAccountImages
CREATE TABLE GameAccountImages (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    GameAccountId INT NOT NULL,
    ImageUrl NVARCHAR(500) NOT NULL,
    IsMainImage BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (GameAccountId) REFERENCES GameAccounts(Id) ON DELETE CASCADE
)
GO

-- Tạo bảng Carts
CREATE TABLE Carts (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
)
GO

-- Tạo bảng CartItems
CREATE TABLE CartItems (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CartId INT NOT NULL,
    GameAccountId INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (CartId) REFERENCES Carts(Id) ON DELETE CASCADE,
    FOREIGN KEY (GameAccountId) REFERENCES GameAccounts(Id)
)
GO

-- Tạo bảng Transactions
CREATE TABLE Transactions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    GameAccountId INT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Pending', -- 'Pending', 'Completed', 'Cancelled'
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CompletedAt DATETIME NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (GameAccountId) REFERENCES GameAccounts(Id)
)
GO

-- Tạo các Indexes
CREATE INDEX IX_Users_Username ON Users(Username)
CREATE INDEX IX_Users_Email ON Users(Email)
CREATE INDEX IX_Users_Role ON Users(Role)
CREATE INDEX IX_GameAccounts_CategoryId ON GameAccounts(CategoryId)
CREATE INDEX IX_GameAccounts_GameType ON GameAccounts(GameType)
CREATE INDEX IX_GameAccounts_Status ON GameAccounts(Status)
CREATE INDEX IX_Carts_UserId ON Carts(UserId)
CREATE INDEX IX_CartItems_CartId ON CartItems(CartId)
CREATE INDEX IX_CartItems_GameAccountId ON CartItems(GameAccountId)
CREATE INDEX IX_Transactions_Status ON Transactions(Status)
GO

-- Tạo Constraints
ALTER TABLE CartItems
ADD CONSTRAINT UQ_CartItems_GameAccount UNIQUE (CartId, GameAccountId)
GO

-- Insert dữ liệu mẫu cho Categories
INSERT INTO Categories (Name, Description)
VALUES 
    (N'Tài khoản loại 1', N'Tài khoản vip'),
    (N'Tài khoản loại 1', N'Tài khoản tầm trung')
GO

-- Insert dữ liệu mẫu cho admin
INSERT INTO Users (Username, Email, PasswordHash, Role, Balance)
VALUES ('admin', 'admin@example.com', '070604', 'Admin', 100000)
GO