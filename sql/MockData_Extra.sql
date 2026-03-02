-- ============================================================
-- BookStation MockData_Extra.sql
-- Additional test data for remaining book-related tables:
--   - BookVariants
--   - Inventories
--   - BookAuthors
--   - BookCategories
--
-- Prerequisite: run docs/MockData.sql first so that
--   Categories, Publishers, Authors, and Books already have data.
-- ============================================================
SET NOCOUNT ON;

/* ====================== BOOK VARIANTS ====================== */
-- One paperback variant for every existing book that does not yet have variants
INSERT INTO BookVariants (
    BookId,
    VariantName,
    Price,
    PriceCurrency,
    OriginalPrice,
    OriginalPriceCurrency,
    WeightGrams,
    SKU,
    IsAvailable,
    CreatedAt,
    UpdatedAt
)
SELECT
    b.Id AS BookId,
    'Paperback' AS VariantName,
    CAST(9.99 + (ROW_NUMBER() OVER (ORDER BY b.Id) * 0.50) AS decimal(18,2)) AS Price,
    'USD' AS PriceCurrency,
    NULL AS OriginalPrice,
    NULL AS OriginalPriceCurrency,
    250 + (b.Id % 300) AS WeightGrams,
    CONCAT('BK', RIGHT('0000' + CAST(b.Id AS varchar(4)), 4), '-PB') AS SKU,
    1 AS IsAvailable,
    GETUTCDATE() AS CreatedAt,
    NULL AS UpdatedAt
FROM Books b
WHERE NOT EXISTS (
    SELECT 1 FROM BookVariants v WHERE v.BookId = b.Id
);

-- A hardcover variant for the first 100 books (if not already present)
INSERT INTO BookVariants (
    BookId,
    VariantName,
    Price,
    PriceCurrency,
    OriginalPrice,
    OriginalPriceCurrency,
    WeightGrams,
    SKU,
    IsAvailable,
    CreatedAt,
    UpdatedAt
)
SELECT TOP (100)
    b.Id AS BookId,
    'Hardcover' AS VariantName,
    CAST(14.99 + (ROW_NUMBER() OVER (ORDER BY b.Id) * 0.75) AS decimal(18,2)) AS Price,
    'USD' AS PriceCurrency,
    NULL AS OriginalPrice,
    NULL AS OriginalPriceCurrency,
    400 + (b.Id % 400) AS WeightGrams,
    CONCAT('BK', RIGHT('0000' + CAST(b.Id AS varchar(4)), 4), '-HC') AS SKU,
    1 AS IsAvailable,
    GETUTCDATE() AS CreatedAt,
    NULL AS UpdatedAt
FROM Books b
WHERE NOT EXISTS (
    SELECT 1 FROM BookVariants v
    WHERE v.BookId = b.Id AND v.VariantName = 'Hardcover'
)
ORDER BY b.Id;


/* ====================== INVENTORIES ====================== */
-- One inventory row per variant if not already present
INSERT INTO Inventories (
    BookVariantId,
    QuantityInStock,
    ReservedQuantity,
    CreatedAt,
    UpdatedAt
)
SELECT
    v.Id AS BookVariantId,
    20 + (v.Id % 80) AS QuantityInStock,
    0 AS ReservedQuantity,
    GETUTCDATE() AS CreatedAt,
    NULL AS UpdatedAt
FROM BookVariants v
WHERE NOT EXISTS (
    SELECT 1 FROM Inventories i WHERE i.BookVariantId = v.Id
);


/* ====================== BOOK AUTHORS ====================== */
-- Link first 200 books with first 200 authors (1:1 by row number) if not already linked
;WITH BooksRanked AS (
    SELECT TOP (200)
           Id,
           ROW_NUMBER() OVER (ORDER BY Id) AS rn
    FROM Books
    ORDER BY Id
),
AuthorsRanked AS (
    SELECT TOP (200)
           Id,
           ROW_NUMBER() OVER (ORDER BY Id) AS rn
    FROM Authors
    ORDER BY Id
)
INSERT INTO BookAuthors (BookId, AuthorId, DisplayOrder)
SELECT
    b.Id AS BookId,
    a.Id AS AuthorId,
    1 AS DisplayOrder
FROM BooksRanked b
JOIN AuthorsRanked a ON a.rn = b.rn
WHERE NOT EXISTS (
    SELECT 1
    FROM BookAuthors ba
    WHERE ba.BookId = b.Id AND ba.AuthorId = a.Id
);


/* ====================== BOOK CATEGORIES ====================== */
-- Assign each book 1–2 categories in a round-robin fashion
;WITH BookList AS (
    SELECT Id, ROW_NUMBER() OVER (ORDER BY Id) AS rn
    FROM Books
),
CategoryList AS (
    SELECT Id, COUNT(*) OVER () AS TotalCount
    FROM Categories
)
INSERT INTO BookCategories (BookId, CategoryId)
SELECT
    b.Id AS BookId,
    1 + ((b.rn - 1) % c.TotalCount) AS CategoryId
FROM BookList b
CROSS JOIN (SELECT TOP 1 TotalCount FROM CategoryList) c
WHERE NOT EXISTS (
    SELECT 1 FROM BookCategories bc
    WHERE bc.BookId = b.Id
);

-- Add a second category for diversity (different modulo pattern)
INSERT INTO BookCategories (BookId, CategoryId)
SELECT
    b.Id AS BookId,
    1 + ((b.rn * 3) % c.TotalCount) AS CategoryId
FROM BookList b
CROSS JOIN (SELECT TOP 1 TotalCount FROM CategoryList) c
WHERE NOT EXISTS (
    SELECT 1 FROM BookCategories bc
    WHERE bc.BookId = b.Id
          AND bc.CategoryId = 1 + ((b.rn * 3) % c.TotalCount)
);


PRINT 'MockData_Extra.sql completed: BookVariants, Inventories, BookAuthors, BookCategories.';

