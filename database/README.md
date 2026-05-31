# Database Queries - ProductManagement

## Schema Overview

The ProductManagement database uses a hierarchical structure where Products contain Packages, Packages can contain sub-Packages (self-referencing), and Items are assigned to Packages through a many-to-many relationship.

### Tables

| Table | Description |
|-------|-------------|
| **Users** | Application users for authentication |
| **Products** | Top-level products with name and price |
| **PackageTypes** | Lookup table for package categories (Box, Crate, Pallet) |
| **Packages** | Packages belonging to a product; supports hierarchy via ParentPackageId |
| **Items** | Individual items that can be assigned to packages |
| **PackageItems** | Join table linking Items to Packages (many-to-many) |
| **AuditLogs** | Tracks all Add, Edit, Delete operations |

### Entity-Relationship Diagram

```
┌──────────────┐       ┌───────────────────┐       ┌────────────────┐
│   Products   │       │     Packages      │       │ PackageTypes   │
├──────────────┤       ├───────────────────┤       ├────────────────┤
│ ProductId PK │◄──────│ ProductId FK      │       │PackageTypeId PK│
│ ProductName  │       │ PackageId PK      │──────►│PackageTypeName │
│ ProductPrice │       │ ParentPackageId FK│──┐    │ CreatedBy      │
│ CreatedBy    │       │ PackageTypeId FK  │  │    │ CreatedDate    │
│ CreatedDate  │       │ CreatedBy         │  │    │ IsDeleted      │
│ IsDeleted    │       │ CreatedDate       │  │    └────────────────┘
└──────────────┘       │ IsDeleted         │  │
                       └───────────────────┘  │
                              │    ▲          │
                              │    └──────────┘
                              │   (self-referencing
                              │    hierarchy)
                              │
                       ┌──────────────────┐
                       │  PackageItems    │       ┌──────────────┐
                       ├──────────────────┤       │    Items     │
                       │ PackageItemId PK │       ├──────────────┤
                       │ PackageId FK     │       │ ItemId PK    │
                       │ ItemId FK        │──────►│ ItemName     │
                       │ CreatedBy        │       │ CreatedBy    │
                       │ CreatedDate      │       │ CreatedDate  │
                       │ IsDeleted        │       │ IsDeleted    │
                       └──────────────────┘       └──────────────┘
```

### Relationships

- **Product → Packages**: One-to-Many (a product has many packages)
- **Package → Packages**: Self-referencing One-to-Many (a package can have sub-packages via ParentPackageId)
- **Package → PackageType**: Many-to-One (each package has one type)
- **Package ↔ Items**: Many-to-Many (through PackageItems join table)

### Design Choices

1. **Soft Delete**: All entity tables use an `IsDeleted` column instead of hard deletes. This preserves data integrity and allows recovery.

2. **Self-Referencing Hierarchy**: The `Packages` table uses `ParentPackageId` (nullable FK to itself) to support unlimited nesting depth. Root packages have `ParentPackageId = NULL`.

3. **Restrict Delete on Hierarchy**: The FK constraint on `ParentPackageId` uses `ON DELETE NO ACTION` to prevent accidental cascade deletion of child packages.

4. **Audit Trail**: The `AuditLogs` table records every mutation with the action type, affected entity, and the user who performed it.

## SQL Scripts

| File | Description |
|------|-------------|
| `01_CreateTables.sql` | Creates all tables, relationships, constraints, and indexes |
| `02_SampleData.sql` | Inserts sample data for testing |
| `03_Queries.sql` | Queries to fetch product-packaging-item relationships using CTEs |

## Query Descriptions

### 03_Queries.sql

1. **Get all products with packages and items** — Flat join showing all relationships
2. **Recursive CTE: Package hierarchy** — Shows all packages and sub-packages with depth level and hierarchy path
3. **Recursive CTE: All items in a product's hierarchy** — Traverses the full package tree to find all items at every level
4. **Product summary** — Package count and item count per product
5. **Recursive CTE: Item path (bottom-up)** — Traces an item's full path back up through the package hierarchy to the product

## Indexing Strategy

- **Filtered indexes on IsDeleted**: Since most queries filter `WHERE IsDeleted = 0`, filtered indexes on the soft delete column improve performance by only indexing active records.
- **Foreign key indexes**: Indexes on `ProductId`, `ParentPackageId`, `PackageTypeId`, `PackageId`, and `ItemId` optimize JOIN operations.
- **Audit log indexes**: Composite index on `(EntityName, EntityId)` for entity-specific lookups, and descending index on `Timestamp` for recent activity queries.
