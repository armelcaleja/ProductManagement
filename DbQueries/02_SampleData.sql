-- Sample Data Insertion

-- 1. Insert User
INSERT INTO Users (Username, Password)
VALUES ('armelcaleja', '$2a$11$examplehashedpassword');

-- 2. Insert Package Types
INSERT INTO PackageTypes (PackageTypeName, CreatedBy)
VALUES 
    ('Box', 1),
    ('Crate', 1),
    ('Pallet', 1);

-- 3. Insert Products
INSERT INTO Products (ProductName, ProductPrice, CreatedBy)
VALUES 
    ('Product Alpha', 100, 1),
    ('Product Beta', 250, 1),
    ('Product Gamma', 500, 1);

-- 4. Insert Root Packages (no parent)
INSERT INTO Packages (ProductId, ParentPackageId, PackageTypeId, CreatedBy)
VALUES 
    (1, NULL, 1, 1),  -- Root package for Product Alpha, type Box
    (2, NULL, 3, 1);  -- Root package for Product Beta, type Pallet

-- 5. Insert Child Packages (referencing parent packages)
INSERT INTO Packages (ProductId, ParentPackageId, PackageTypeId, CreatedBy)
VALUES 
    (1, 1, 2, 1),  -- Child of package 1, type Crate
    (1, 1, 1, 1),  -- Child of package 1, type Box
    (2, 2, 2, 1);  -- Child of package 2, type Crate

-- 6. Insert Items
INSERT INTO Items (ItemName, CreatedBy)
VALUES 
    ('Widget A', 1),
    ('Widget B', 1),
    ('Gadget X', 1),
    ('Gadget Y', 1),
    ('Component Z', 1),
    ('Part W', 1);

-- 7. Assign Items to Packages
INSERT INTO PackageItems (PackageId, ItemId, CreatedBy)
VALUES 
    (1, 1, 1),  -- Widget A -> Root Package 1
    (1, 2, 1),  -- Widget B -> Root Package 1
    (3, 3, 1),  -- Gadget X -> Child Package 3
    (4, 4, 1),  -- Gadget Y -> Child Package 4
    (2, 5, 1),  -- Component Z -> Root Package 2
    (5, 6, 1);  -- Part W -> Child Package 5
