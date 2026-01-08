-- SampleProject 資料庫建立腳本
-- 使用 Dapper 時需要手動建立資料表和結構

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'SampleProjectDb')
BEGIN
    CREATE DATABASE SampleProjectDb;
END
GO

USE SampleProjectDb;
GO

-- 建立 Products 資料表 (SPU)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Products')
BEGIN
    CREATE TABLE Products (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ProductId UNIQUEIDENTIFIER NOT NULL UNIQUE,
        Name NVARCHAR(200) NOT NULL,
        Description NVARCHAR(1000) NULL,
        BasePrice DECIMAL(18,2) NOT NULL,
        CreatedAt DATETIME2 NOT NULL,
        UpdatedAt DATETIME2 NULL
    );

    -- 建立索引以提升查詢效能
    CREATE INDEX IX_Products_CreatedAt ON Products(CreatedAt);
    CREATE UNIQUE INDEX IX_Products_ProductId ON Products(ProductId);
END
GO

-- 建立 Skus 資料表 (SKU)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Skus')
BEGIN
    CREATE TABLE Skus (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        SkuId UNIQUEIDENTIFIER NOT NULL UNIQUE,
        ProductId INT NOT NULL,
        SkuCode NVARCHAR(50) NOT NULL UNIQUE,
        Price DECIMAL(18,2) NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL,
        UpdatedAt DATETIME2 NULL,
        CONSTRAINT FK_Skus_Products FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE
    );

    CREATE INDEX IX_Skus_ProductId ON Skus(ProductId);
    CREATE UNIQUE INDEX IX_Skus_SkuId ON Skus(SkuId);
    CREATE UNIQUE INDEX IX_Skus_SkuCode ON Skus(SkuCode);
END
GO

-- 建立 Storages 資料表 (倉庫)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Storages')
BEGIN
    CREATE TABLE Storages (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        StorageId UNIQUEIDENTIFIER NOT NULL UNIQUE,
        Name NVARCHAR(200) NOT NULL,
        Address NVARCHAR(500) NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL,
        UpdatedAt DATETIME2 NULL
    );

    CREATE UNIQUE INDEX IX_Storages_StorageId ON Storages(StorageId);
    CREATE INDEX IX_Storages_IsActive ON Storages(IsActive);
END
GO

-- 建立 Specifications 資料表 (規格定義)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Specifications')
BEGIN
    CREATE TABLE Specifications (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        SpecificationId UNIQUEIDENTIFIER NOT NULL UNIQUE,
        Name NVARCHAR(100) NOT NULL UNIQUE,
        DisplayOrder INT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL,
        UpdatedAt DATETIME2 NULL
    );

    CREATE UNIQUE INDEX IX_Specifications_SpecificationId ON Specifications(SpecificationId);
    CREATE UNIQUE INDEX IX_Specifications_Name ON Specifications(Name);
END
GO

-- 建立 SpecificationValues 資料表 (規格值)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SpecificationValues')
BEGIN
    CREATE TABLE SpecificationValues (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        SpecificationValueId UNIQUEIDENTIFIER NOT NULL UNIQUE,
        SpecificationId INT NOT NULL,
        Value NVARCHAR(100) NOT NULL,
        DisplayOrder INT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL,
        UpdatedAt DATETIME2 NULL,
        CONSTRAINT FK_SpecificationValues_Specifications FOREIGN KEY (SpecificationId) REFERENCES Specifications(Id) ON DELETE CASCADE
    );

    CREATE INDEX IX_SpecificationValues_SpecificationId ON SpecificationValues(SpecificationId);
    CREATE UNIQUE INDEX IX_SpecificationValues_SpecificationValueId ON SpecificationValues(SpecificationValueId);
END
GO

-- 建立 SkuSpecifications 資料表 (SKU-規格值關聯)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SkuSpecifications')
BEGIN
    CREATE TABLE SkuSpecifications (
        SkuId INT NOT NULL,
        SpecificationValueId INT NOT NULL,
        CONSTRAINT PK_SkuSpecifications PRIMARY KEY (SkuId, SpecificationValueId),
        CONSTRAINT FK_SkuSpecifications_Skus FOREIGN KEY (SkuId) REFERENCES Skus(Id) ON DELETE CASCADE,
        CONSTRAINT FK_SkuSpecifications_SpecificationValues FOREIGN KEY (SpecificationValueId) REFERENCES SpecificationValues(Id) ON DELETE CASCADE
    );

    CREATE INDEX IX_SkuSpecifications_SkuId ON SkuSpecifications(SkuId);
    CREATE INDEX IX_SkuSpecifications_SpecificationValueId ON SkuSpecifications(SpecificationValueId);
END
GO

-- 建立 Inventories 資料表 (庫存)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Inventories')
BEGIN
    CREATE TABLE Inventories (
        SkuId INT NOT NULL,
        StorageId INT NOT NULL,
        Quantity INT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL,
        UpdatedAt DATETIME2 NULL,
        CONSTRAINT PK_Inventories PRIMARY KEY (SkuId, StorageId),
        CONSTRAINT FK_Inventories_Skus FOREIGN KEY (SkuId) REFERENCES Skus(Id) ON DELETE CASCADE,
        CONSTRAINT FK_Inventories_Storages FOREIGN KEY (StorageId) REFERENCES Storages(Id) ON DELETE CASCADE
    );

    CREATE INDEX IX_Inventories_SkuId ON Inventories(SkuId);
    CREATE INDEX IX_Inventories_StorageId ON Inventories(StorageId);
END
GO

-- ============================================
-- 預設規格資料初始化
-- ============================================

-- 建立 Size 規格（如果不存在）
IF NOT EXISTS (SELECT 1 FROM Specifications WHERE Name = 'Size')
BEGIN
    DECLARE @SizeSpecId INT;

    INSERT INTO Specifications (SpecificationId, Name, DisplayOrder, CreatedAt)
    VALUES (NEWID(), 'Size', 1, GETUTCDATE());

    SET @SizeSpecId = SCOPE_IDENTITY();

    INSERT INTO SpecificationValues (SpecificationValueId, SpecificationId, Value, DisplayOrder, CreatedAt)
    VALUES
        (NEWID(), @SizeSpecId, 'XS', 1, GETUTCDATE()),
        (NEWID(), @SizeSpecId, 'S', 2, GETUTCDATE()),
        (NEWID(), @SizeSpecId, 'M', 3, GETUTCDATE()),
        (NEWID(), @SizeSpecId, 'L', 4, GETUTCDATE()),
        (NEWID(), @SizeSpecId, 'XL', 5, GETUTCDATE()),
        (NEWID(), @SizeSpecId, 'XXL', 6, GETUTCDATE());
END
GO

-- 建立 Color 規格（如果不存在）
IF NOT EXISTS (SELECT 1 FROM Specifications WHERE Name = 'Color')
BEGIN
    DECLARE @ColorSpecId INT;

    INSERT INTO Specifications (SpecificationId, Name, DisplayOrder, CreatedAt)
    VALUES (NEWID(), 'Color', 2, GETUTCDATE());

    SET @ColorSpecId = SCOPE_IDENTITY();

    INSERT INTO SpecificationValues (SpecificationValueId, SpecificationId, Value, DisplayOrder, CreatedAt)
    VALUES
        (NEWID(), @ColorSpecId, 'Black', 1, GETUTCDATE()),
        (NEWID(), @ColorSpecId, 'White', 2, GETUTCDATE()),
        (NEWID(), @ColorSpecId, 'Blue', 3, GETUTCDATE()),
        (NEWID(), @ColorSpecId, 'Red', 4, GETUTCDATE()),
        (NEWID(), @ColorSpecId, 'Gray', 5, GETUTCDATE()),
        (NEWID(), @ColorSpecId, 'Green', 6, GETUTCDATE());
END
GO

-- 建立 Capacity 規格（如果不存在）
IF NOT EXISTS (SELECT 1 FROM Specifications WHERE Name = 'Capacity')
BEGIN
    DECLARE @CapacitySpecId INT;

    INSERT INTO Specifications (SpecificationId, Name, DisplayOrder, CreatedAt)
    VALUES (NEWID(), 'Capacity', 3, GETUTCDATE());

    SET @CapacitySpecId = SCOPE_IDENTITY();

    INSERT INTO SpecificationValues (SpecificationValueId, SpecificationId, Value, DisplayOrder, CreatedAt)
    VALUES
        (NEWID(), @CapacitySpecId, '64GB', 1, GETUTCDATE()),
        (NEWID(), @CapacitySpecId, '128GB', 2, GETUTCDATE()),
        (NEWID(), @CapacitySpecId, '256GB', 3, GETUTCDATE()),
        (NEWID(), @CapacitySpecId, '512GB', 4, GETUTCDATE()),
        (NEWID(), @CapacitySpecId, '1TB', 5, GETUTCDATE());
END
GO

-- 建立 Weight 規格（如果不存在）
IF NOT EXISTS (SELECT 1 FROM Specifications WHERE Name = 'Weight')
BEGIN
    DECLARE @WeightSpecId INT;

    INSERT INTO Specifications (SpecificationId, Name, DisplayOrder, CreatedAt)
    VALUES (NEWID(), 'Weight', 4, GETUTCDATE());

    SET @WeightSpecId = SCOPE_IDENTITY();

    INSERT INTO SpecificationValues (SpecificationValueId, SpecificationId, Value, DisplayOrder, CreatedAt)
    VALUES
        (NEWID(), @WeightSpecId, '200g', 1, GETUTCDATE()),
        (NEWID(), @WeightSpecId, '500g', 2, GETUTCDATE()),
        (NEWID(), @WeightSpecId, '1kg', 3, GETUTCDATE()),
        (NEWID(), @WeightSpecId, '2kg', 4, GETUTCDATE());
END
GO
