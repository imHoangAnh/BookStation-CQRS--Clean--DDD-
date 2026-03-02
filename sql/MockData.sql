-- ============================================================
-- BookStation MockData.sql
-- Simple seed for Category, Publisher, Author, Book
-- - Categories: 12 rows
-- - Publishers: 10 rows
-- - Authors: 300 rows (generated from sys.objects)
-- - Books: 300 rows (generated from sys.objects)
-- Run this in SSMS 2022 against the BookStation database.
-- ============================================================
SET NOCOUNT ON;

/* ====================== CATEGORIES ====================== */
INSERT INTO Categories (Name, Description, ParentCategoryId, CreatedAt, UpdatedAt) VALUES
('Fiction', 'Novels and imaginative stories', NULL, GETUTCDATE(), NULL),
('Non-Fiction', 'Biographies, history, and essays', NULL, GETUTCDATE(), NULL),
('Science Fiction', 'Speculative and futuristic fiction', NULL, GETUTCDATE(), NULL),
('Fantasy', 'Magic, mythical worlds, and epic tales', NULL, GETUTCDATE(), NULL),
('Mystery & Thriller', 'Crime, mystery, and suspense', NULL, GETUTCDATE(), NULL),
('Romance', 'Love stories and relationships', NULL, GETUTCDATE(), NULL),
('Young Adult', 'Stories for young adult readers', NULL, GETUTCDATE(), NULL),
('Children', 'Books for children and early readers', NULL, GETUTCDATE(), NULL),
('Business', 'Management, leadership, and finance', NULL, GETUTCDATE(), NULL),
('Technology', 'Programming, IT, and engineering', NULL, GETUTCDATE(), NULL),
('Self-Help', 'Personal growth and productivity', NULL, GETUTCDATE(), NULL),
('Science', 'Popular science and nature', NULL, GETUTCDATE(), NULL);


/* ====================== PUBLISHERS ====================== */
INSERT INTO Publishers (Id, Name, Description, Address, Website, LogoUrl, IsActive, CreatedAt, UpdatedAt) VALUES
(NEWID(), 'Northwind Press', 'Independent fiction and non-fiction publisher', '1200 Maple Street, Seattle, WA', 'https://northwind.example.com', NULL, 1, GETUTCDATE(), NULL),
(NEWID(), 'Evergreen Books', 'Publisher focused on literary fiction', '55 River Road, Portland, OR', 'https://evergreen.example.com', NULL, 1, GETUTCDATE(), NULL),
(NEWID(), 'Aurora House', 'Science fiction and fantasy imprint', '9 Aurora Plaza, Denver, CO', 'https://aurorahouse.example.com', NULL, 1, GETUTCDATE(), NULL),
(NEWID(), 'Insight Media', 'Business, technology, and self-help titles', '800 Innovation Way, San Jose, CA', 'https://insightmedia.example.com', NULL, 1, GETUTCDATE(), NULL),
(NEWID(), 'Blue Harbor Publishing', 'Travel, nature, and lifestyle books', '42 Harbor Lane, Boston, MA', 'https://blueharbor.example.com', NULL, 1, GETUTCDATE(), NULL),
(NEWID(), 'Silver Leaf Press', 'Poetry and contemporary literature', '17 Poet Avenue, Chicago, IL', 'https://silverleaf.example.com', NULL, 1, GETUTCDATE(), NULL),
(NEWID(), 'BrightMind Education', 'Educational and reference materials', '300 Campus Drive, Austin, TX', 'https://brightmind.example.com', NULL, 1, GETUTCDATE(), NULL),
(NEWID(), 'Atlas Editions', 'History and biography collection', '88 Heritage Street, New York, NY', 'https://atlaseditions.example.com', NULL, 1, GETUTCDATE(), NULL),
(NEWID(), 'PixelWorks Publishing', 'Programming and software engineering books', '500 Code Park, San Francisco, CA', 'https://pixelworks.example.com', NULL, 1, GETUTCDATE(), NULL),
(NEWID(), 'Starlight Kids', 'Children and young readers imprint', '12 Storybook Road, Orlando, FL', 'https://starlightkids.example.com', NULL, 1, GETUTCDATE(), NULL);


/* ====================== AUTHORS (~300) ====================== */
;WITH AuthorNumbers AS (
    SELECT TOP (300)
           ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n
    FROM sys.objects
)
INSERT INTO Authors (Id, FullName, Bio, DateOfBirth, DiedDate, Address, Country, PhotoUrl, UserId, CreatedAt, UpdatedAt)
SELECT
    NEWID() AS Id,
    CONCAT('Author ', n) AS FullName,
    CONCAT('Sample author #', n, ' used for testing data.') AS Bio,
    NULL AS DateOfBirth,
    NULL AS DiedDate,
    NULL AS Address,
    CASE (n % 6)
        WHEN 0 THEN 'United States'
        WHEN 1 THEN 'United Kingdom'
        WHEN 2 THEN 'Canada'
        WHEN 3 THEN 'Australia'
        WHEN 4 THEN 'Germany'
        ELSE 'France'
    END AS Country,
    NULL AS PhotoUrl,
    NULL AS UserId,
    GETUTCDATE() AS CreatedAt,
    NULL AS UpdatedAt
FROM AuthorNumbers;


/* ====================== BOOKS (~300) ====================== */
;WITH BookNumbers AS (
    SELECT TOP (300)
           ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n
    FROM sys.objects
)
INSERT INTO Books (
    Title,
    ISBN,
    Description,
    Language,
    PublishYear,
    PublisherId,
    SellerId,
    Status,
    CoverImageUrl,
    PageCount,
    CoAuthors,
    Translators,
    CreatedAt,
    UpdatedAt
)
SELECT
    CONCAT('Sample Book ', n) AS Title,
    RIGHT('9780000000000' + CAST(n AS varchar(10)), 13) AS ISBN,
    CONCAT('Sample description for book ', n, '. This is English test data for development and QA.') AS Description,
    'English' AS Language,
    1985 + (n % 40) AS PublishYear,      -- years roughly between 1985 and 2024
    NULL AS PublisherId,                 -- keep NULL to avoid FK issues if Publishers differ
    NULL AS SellerId,
    'Active' AS Status,
    NULL AS CoverImageUrl,
    150 + (n % 450) AS PageCount,        -- between ~150 and ~599 pages
    NULL AS CoAuthors,
    NULL AS Translators,
    GETUTCDATE() AS CreatedAt,
    NULL AS UpdatedAt
FROM BookNumbers;


PRINT 'MockData.sql seed completed: 12 Categories, 10 Publishers, 300 Authors, 300 Books.';

