-- ============================================
-- Product -> Package/Item Hierarchy
-- ============================================
;WITH PackageHierarchy AS (
    -- Anchor: root packages (no parent)
    SELECT 
        pkg.PackageId,
        pkg.ProductId,
        pkg.ParentPackageId,
        pkg.PackageTypeId,
        0 AS HierarchyLevel
    FROM Packages pkg
    WHERE pkg.ParentPackageId IS NULL
      AND pkg.IsDeleted = 0

    UNION ALL

    -- Recursive: child packages
    SELECT 
        child.PackageId,
        child.ProductId,
        child.ParentPackageId,
        child.PackageTypeId,
        parent.HierarchyLevel + 1
    FROM Packages child
    INNER JOIN PackageHierarchy parent ON child.ParentPackageId = parent.PackageId
    WHERE child.IsDeleted = 0
)
-- Package rows
SELECT 
    p.ProductId,
    p.ProductName,
    ph.PackageId,
    NULL AS ItemId,
    pt.PackageTypeName,
    NULL AS ItemName,
    ph.ParentPackageId,
    ph.HierarchyLevel
FROM PackageHierarchy ph
INNER JOIN Products p ON p.ProductId = ph.ProductId AND p.IsDeleted = 0
INNER JOIN PackageTypes pt ON pt.PackageTypeId = ph.PackageTypeId AND pt.IsDeleted = 0

UNION ALL

-- Item rows
SELECT 
    p.ProductId,
    p.ProductName,
    NULL AS PackageId,
    i.ItemId,
    NULL AS PackageTypeName,
    i.ItemName,
    pi.PackageId AS ParentPackageId,
    ph.HierarchyLevel + 1
FROM PackageHierarchy ph
INNER JOIN Products p ON p.ProductId = ph.ProductId AND p.IsDeleted = 0
INNER JOIN PackageItems pi ON pi.PackageId = ph.PackageId AND pi.IsDeleted = 0
INNER JOIN Items i ON i.ItemId = pi.ItemId AND i.IsDeleted = 0

ORDER BY ProductName, HierarchyLevel, PackageId, ItemId;
