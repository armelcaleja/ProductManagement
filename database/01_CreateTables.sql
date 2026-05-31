-- ============================================
-- ProductManagement Database Schema
-- SQL Script to create tables, relationships, and constraints
-- ============================================

-- 1. Users Table
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(MAX) NOT NULL,
    Password NVARCHAR(MAX) NOT NULL
);

-- 2. PackageTypes Table
CREATE TABLE PackageTypes (
    PackageTypeId INT IDENTITY(1,1) PRIMARY KEY,
    PackageTypeName NVARCHAR(MAX) NOT NULL,
    CreatedBy INT NOT NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0
);

-- 3. Products Table
CREATE TABLE Products (
    ProductId INT IDENTITY(1,1) PRIMARY KEY,
    ProductName NVARCHAR(MAX) NOT NULL,
    ProductPrice INT NOT NULL,
    CreatedBy INT NOT NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0
);

-- 4. Items Table
CREATE TABLE Items (
    ItemId INT IDENTITY(1,1) PRIMARY KEY,
    ItemName NVARCHAR(MAX) NOT NULL,
    CreatedBy INT NOT NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0
);

-- 5. Packages Table (self-referencing hierarchy)
CREATE TABLE Packages (
    PackageId INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT NOT NULL,
    ParentPackageId INT NULL,
    PackageTypeId INT NOT NULL,
    CreatedBy INT NOT NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_Packages_Products FOREIGN KEY (ProductId) REFERENCES Products(ProductId),
    CONSTRAINT FK_Packages_PackageTypes FOREIGN KEY (PackageTypeId) REFERENCES PackageTypes(PackageTypeId),
    CONSTRAINT FK_Packages_ParentPackage FOREIGN KEY (ParentPackageId) REFERENCES Packages(PackageId) ON DELETE NO ACTION
);

-- 6. PackageItems Table (many-to-many between Packages and Items)
CREATE TABLE PackageItems (
    PackageItemId INT IDENTITY(1,1) PRIMARY KEY,
    PackageId INT NOT NULL,
    ItemId INT NOT NULL,
    CreatedBy INT NOT NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_PackageItems_Packages FOREIGN KEY (PackageId) REFERENCES Packages(PackageId),
    CONSTRAINT FK_PackageItems_Items FOREIGN KEY (ItemId) REFERENCES Items(ItemId)
);

-- 7. AuditLogs Table
CREATE TABLE AuditLogs (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Action NVARCHAR(MAX) NOT NULL,
    EntityName NVARCHAR(MAX) NOT NULL,
    EntityId INT NOT NULL,
    Details NVARCHAR(MAX) NOT NULL,
    PerformedBy INT NOT NULL,
    Timestamp DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- ============================================
-- Indexes for optimized performance
-- ============================================

-- Soft delete filter indexes (most queries filter by IsDeleted = 0)
CREATE NONCLUSTERED INDEX IX_Products_IsDeleted ON Products(IsDeleted) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Items_IsDeleted ON Items(IsDeleted) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Packages_IsDeleted ON Packages(IsDeleted) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_PackageTypes_IsDeleted ON PackageTypes(IsDeleted) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_PackageItems_IsDeleted ON PackageItems(IsDeleted) WHERE IsDeleted = 0;

-- Foreign key indexes for join performance
CREATE NONCLUSTERED INDEX IX_Packages_ProductId ON Packages(ProductId);
CREATE NONCLUSTERED INDEX IX_Packages_ParentPackageId ON Packages(ParentPackageId);
CREATE NONCLUSTERED INDEX IX_Packages_PackageTypeId ON Packages(PackageTypeId);
CREATE NONCLUSTERED INDEX IX_PackageItems_PackageId ON PackageItems(PackageId);
CREATE NONCLUSTERED INDEX IX_PackageItems_ItemId ON PackageItems(ItemId);

-- Audit log lookup
CREATE NONCLUSTERED INDEX IX_AuditLogs_EntityName_EntityId ON AuditLogs(EntityName, EntityId);
CREATE NONCLUSTERED INDEX IX_AuditLogs_Timestamp ON AuditLogs(Timestamp DESC);
