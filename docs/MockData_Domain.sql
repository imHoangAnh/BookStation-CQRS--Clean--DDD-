-- ============================================================
-- BookStation MockData_Domain.sql
-- Extra domain test data for remaining aggregates:
--   - Users, SellerProfiles, AddressWallets
--   - Carts, CartItems
--   - Orders, OrderItems, Payments
--
-- Prerequisite (recommended):
--   1) Run docs/MockData.sql        (Categories, Publishers, Authors, Books)
--   2) Run docs/MockData_Extra.sql  (BookVariants, Inventories, BookAuthors, BookCategories)
--
-- Then run this file.
-- All data is English-only and for development/testing.
-- ============================================================
SET NOCOUNT ON;


/* ====================== USERS (10) ====================== */
-- Simple test users (no password policy enforced here)
INSERT INTO Users (
    Id,
    Email,
    FullName,
    PhoneNumber,
    PasswordHash,
    IsVerified,
    Status,
    DateOfBirth,
    Gender,
    Bio,
    AvatarUrl,
    CreatedAt,
    UpdatedAt
)
VALUES
('11111111-1111-1111-1111-111111111111', 'alice.green@example.com',   'Alice Green',   '+15551000001', 'HASH_A', 1, 'Active', '1992-03-15', 1, 'Test customer', NULL, GETUTCDATE(), GETUTCDATE()),
('22222222-2222-2222-2222-222222222222', 'bob.miller@example.com',    'Bob Miller',    '+15551000002', 'HASH_B', 1, 'Active', '1988-07-02', 0, 'Test customer', NULL, GETUTCDATE(), GETUTCDATE()),
('33333333-3333-3333-3333-333333333333', 'carol.james@example.com',  'Carol James',   '+15551000003', 'HASH_C', 1, 'Active', '1995-11-21', 1, 'Test customer', NULL, GETUTCDATE(), GETUTCDATE()),
('44444444-4444-4444-4444-444444444444', 'david.ross@example.com',   'David Ross',    '+15551000004', 'HASH_D', 1, 'Active', '1990-01-10', 0, 'Test customer', NULL, GETUTCDATE(), GETUTCDATE()),
('55555555-5555-5555-5555-555555555555', 'eva.king@example.com',     'Eva King',      '+15551000005', 'HASH_E', 1, 'Active', '1998-05-30', 1, 'Test customer', NULL, GETUTCDATE(), GETUTCDATE()),
('66666666-6666-6666-6666-666666666666', 'frank.moore@example.com',  'Frank Moore',   '+15551000006', 'HASH_F', 1, 'Active', '1985-09-12', 0, 'Test seller',   NULL, GETUTCDATE(), GETUTCDATE()),
('77777777-7777-7777-7777-777777777777', 'grace.lee@example.com',    'Grace Lee',     '+15551000007', 'HASH_G', 1, 'Active', '1993-02-18', 1, 'Test seller',   NULL, GETUTCDATE(), GETUTCDATE()),
('88888888-8888-8888-8888-888888888888', 'henry.clark@example.com',  'Henry Clark',   '+15551000008', 'HASH_H', 1, 'Active', '1982-12-01', 0, 'Test customer', NULL, GETUTCDATE(), GETUTCDATE()),
('99999999-9999-9999-9999-999999999999', 'irene.adams@example.com',  'Irene Adams',   '+15551000009', 'HASH_I', 1, 'Active', '1991-08-09', 1, 'Test customer', NULL, GETUTCDATE(), GETUTCDATE()),
('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'jack.turner@example.com',  'Jack Turner',   '+15551000010', 'HASH_J', 1, 'Active', '1989-04-27', 0, 'Test customer', NULL, GETUTCDATE(), GETUTCDATE());


/* ====================== SELLER PROFILES (2) ====================== */
-- Simple seller profiles for two users (Frank & Grace)
INSERT INTO SellerProfiles (
    Id,
    Status,
    OrganizationId,
    DateOfBirth,
    Gender,
    Address_Street,
    Address_Ward,
    Address_City,
    Address_Country,
    Address_PostalCode,
    IdNumber,
    ApprovedAt,
    UserId,
    CreatedAt,
    UpdatedAt
)
VALUES
('66666666-6666-6666-6666-666666666666', 'Active', NULL, '1985-09-12', 'Male',   '500 Market Street',    'Downtown', 'Seattle',  'United States', '98101', 'IDF123456', GETUTCDATE(), '66666666-6666-6666-6666-666666666666', GETUTCDATE(), GETUTCDATE()),
('77777777-7777-7777-7777-777777777777', 'Active', NULL, '1993-02-18', 'Female', '120 Ocean Avenue',     'Harbor',   'San Diego','United States', '92101', 'IDG654321', GETUTCDATE(), '77777777-7777-7777-7777-777777777777', GETUTCDATE(), GETUTCDATE());


/* ====================== ADDRESS WALLETS (1–2 mỗi user) ====================== */
-- Sử dụng AddressLabel: 'Home', 'Work', 'Other'
INSERT INTO AddressWallets (
    Id,
    UserId,
    RecipientName,
    RecipientPhone,
    Street,
    Ward,
    City,
    Country,
    PostalCode,
    Label,
    IsDefault,
    CreatedAt,
    UpdatedAt
)
VALUES
(NEWID(), '11111111-1111-1111-1111-111111111111', 'Alice Green',  '+15551000001', '10 Green Street',   'Central',  'Seattle',     'United States', '98101', 'Home',  1, GETUTCDATE(), GETUTCDATE()),
(NEWID(), '11111111-1111-1111-1111-111111111111', 'Alice Green',  '+15551000001', '200 Lake Avenue',   'North',    'Seattle',     'United States', '98105', 'Work',  0, GETUTCDATE(), GETUTCDATE()),
(NEWID(), '22222222-2222-2222-2222-222222222222', 'Bob Miller',   '+15551000002', '45 Hill Road',      'West',     'Portland',    'United States', '97201', 'Home',  1, GETUTCDATE(), GETUTCDATE()),
(NEWID(), '33333333-3333-3333-3333-333333333333', 'Carol James',  '+15551000003', '88 River Lane',     'East',     'Austin',      'United States', '73301', 'Home',  1, GETUTCDATE(), GETUTCDATE()),
(NEWID(), '44444444-4444-4444-4444-444444444444', 'David Ross',   '+15551000004', '19 Pine Street',    'Old Town', 'Denver',      'United States', '80202', 'Home',  1, GETUTCDATE(), GETUTCDATE()),
(NEWID(), '55555555-5555-5555-5555-555555555555', 'Eva King',     '+15551000005', '7 Garden View',     'Central',  'Boston',      'United States', '02108', 'Home',  1, GETUTCDATE(), GETUTCDATE()),
(NEWID(), '66666666-6666-6666-6666-666666666666', 'Frank Moore',  '+15551000006', '500 Market Street', 'Downtown', 'Seattle',     'United States', '98101', 'Home',  1, GETUTCDATE(), GETUTCDATE()),
(NEWID(), '77777777-7777-7777-7777-777777777777', 'Grace Lee',    '+15551000007', '120 Ocean Avenue',  'Harbor',   'San Diego',   'United States', '92101', 'Home',  1, GETUTCDATE(), GETUTCDATE()),
(NEWID(), '88888888-8888-8888-8888-888888888888', 'Henry Clark',  '+15551000008', '30 Sunset Road',    'West',     'Phoenix',     'United States', '85001', 'Home',  1, GETUTCDATE(), GETUTCDATE()),
(NEWID(), '99999999-9999-9999-9999-999999999999', 'Irene Adams',  '+15551000009', '15 Oak Street',     'Central',  'Chicago',     'United States', '60601', 'Home',  1, GETUTCDATE(), GETUTCDATE()),
(NEWID(), 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'Jack Turner',  '+15551000010', '90 Maple Avenue',   'South',    'New York',    'United States', '10001', 'Home',  1, GETUTCDATE(), GETUTCDATE());


/* ====================== CARTS (1 mỗi user) ====================== */
INSERT INTO Carts (Id, UserId, UpdateAt, CreatedAt, UpdatedAt)
VALUES
('c1111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', GETUTCDATE(), GETUTCDATE(), NULL),
('c2222222-2222-2222-2222-222222222222', '22222222-2222-2222-2222-222222222222', GETUTCDATE(), GETUTCDATE(), NULL),
('c3333333-3333-3333-3333-333333333333', '33333333-3333-3333-3333-333333333333', GETUTCDATE(), GETUTCDATE(), NULL),
('c4444444-4444-4444-4444-444444444444', '44444444-4444-4444-4444-444444444444', GETUTCDATE(), GETUTCDATE(), NULL),
('c5555555-5555-5555-5555-555555555555', '55555555-5555-5555-5555-555555555555', GETUTCDATE(), GETUTCDATE(), NULL),
('c6666666-6666-6666-6666-666666666666', '66666666-6666-6666-6666-666666666666', GETUTCDATE(), GETUTCDATE(), NULL),
('c7777777-7777-7777-7777-777777777777', '77777777-7777-7777-7777-777777777777', GETUTCDATE(), GETUTCDATE(), NULL),
('c8888888-8888-8888-8888-888888888888', '88888888-8888-8888-8888-888888888888', GETUTCDATE(), GETUTCDATE(), NULL),
('c9999999-9999-9999-9999-999999999999', '99999999-9999-9999-9999-999999999999', GETUTCDATE(), GETUTCDATE(), NULL),
('caaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', GETUTCDATE(), GETUTCDATE(), NULL);


/* ====================== CART ITEMS (từ variants hiện có) ====================== */
;WITH CartList AS (
    SELECT Id, ROW_NUMBER() OVER (ORDER BY Id) AS rn
    FROM Carts
),
VariantList AS (
    SELECT Id, ROW_NUMBER() OVER (ORDER BY Id) AS rn
    FROM BookVariants
)
INSERT INTO CartItems (BookVariantId, Quantity, CartId, CreatedAt, UpdatedAt)
SELECT
    v.Id AS BookVariantId,
    1 + (c.rn % 3) AS Quantity,
    c.Id AS CartId,
    GETUTCDATE() AS CreatedAt,
    NULL AS UpdatedAt
FROM CartList c
JOIN VariantList v ON v.rn BETWEEN c.rn * 2 - 1 AND c.rn * 2;  -- 2 items per cart if possible


/* ====================== ORDERS (8) ====================== */
-- Lưu ý: Order.UserId là bigint độc lập, không FK tới Users (Guid)
INSERT INTO Orders (
    UserId,
    TotalAmount,
    TotalAmountCurrency,
    DiscountAmount,
    DiscountAmountCurrency,
    FinalAmount,
    FinalAmountCurrency,
    Status,
    VoucherId,
    ShippingStreet,
    ShippingWard,
    ShippingCity,
    ShippingCountry,
    ShippingPostalCode,
    Notes,
    ConfirmedAt,
    CompletedAt,
    CancelledAt,
    CancellationReason,
    CreatedAt,
    UpdatedAt
)
VALUES
(1,  39.98, 'USD', 0.00, 'USD', 39.98, 'USD', 'Delivered', NULL, '10 Green Street',   'Central',  'Seattle',     'United States', '98101', 'Test completed order', DATEADD(DAY,-7,GETUTCDATE()), DATEADD(DAY,-5,GETUTCDATE()), NULL, NULL, DATEADD(DAY,-8,GETUTCDATE()), NULL),
(2,  24.99, 'USD', 5.00, 'USD', 19.99, 'USD', 'Delivered', NULL, '45 Hill Road',      'West',     'Portland',    'United States', '97201', NULL,                  DATEADD(DAY,-10,GETUTCDATE()), DATEADD(DAY,-8,GETUTCDATE()), NULL, NULL, DATEADD(DAY,-11,GETUTCDATE()), NULL),
(3,  59.97, 'USD', 0.00, 'USD', 59.97, 'USD', 'Confirmed', NULL, '88 River Lane',     'East',     'Austin',      'United States', '73301', 'Ready to ship',       DATEADD(DAY,-2,GETUTCDATE()), NULL,                    NULL, NULL, DATEADD(DAY,-3,GETUTCDATE()), NULL),
(4,  14.99, 'USD', 0.00, 'USD', 14.99, 'USD', 'Pending',   NULL, '19 Pine Street',    'Old Town', 'Denver',      'United States', '80202', 'Pending payment',      NULL,                           NULL,                    NULL, NULL, GETUTCDATE(),              NULL),
(5,  89.50, 'USD',10.00, 'USD', 79.50, 'USD', 'Processing',NULL, '7 Garden View',     'Central',  'Boston',      'United States', '02108', NULL,                  DATEADD(DAY,-1,GETUTCDATE()), NULL,                    NULL, NULL, DATEADD(DAY,-2,GETUTCDATE()), NULL),
(6,  27.99, 'USD', 0.00, 'USD', 27.99, 'USD', 'Cancelled', NULL, '500 Market Street', 'Downtown', 'Seattle',     'United States', '98101', 'Customer cancelled',   NULL,                           NULL,                    DATEADD(DAY,-3,GETUTCDATE()), 'Customer changed mind', DATEADD(DAY,-4,GETUTCDATE()), NULL),
(7, 120.00, 'USD',20.00, 'USD',100.00, 'USD', 'Shipped',   NULL, '120 Ocean Avenue',  'Harbor',   'San Diego',   'United States', '92101', NULL,                  DATEADD(DAY,-4,GETUTCDATE()), NULL,                    NULL, NULL, DATEADD(DAY,-5,GETUTCDATE()), NULL),
(8,  49.99, 'USD', 0.00, 'USD', 49.99, 'USD', 'Returned',  NULL, '90 Maple Avenue',   'South',    'New York',    'United States', '10001', 'Returned by customer',DATEADD(DAY,-9,GETUTCDATE()), DATEADD(DAY,-7,GETUTCDATE()), NULL, NULL, DATEADD(DAY,-10,GETUTCDATE()), NULL);


/* ====================== ORDER ITEMS (dựa trên BookVariants) ====================== */
;WITH OrderList AS (
    SELECT Id, ROW_NUMBER() OVER (ORDER BY Id) AS rn
    FROM Orders
),
VariantList AS (
    SELECT Id, BookId, ROW_NUMBER() OVER (ORDER BY Id) AS rn
    FROM BookVariants
)
INSERT INTO OrderItems (
    OrderId,
    BookVariantId,
    Quantity,
    UnitPrice,
    UnitPriceCurrency,
    BookTitle,
    VariantName,
    CreatedAt,
    UpdatedAt
)
SELECT
    o.Id AS OrderId,
    v.Id AS BookVariantId,
    1 + (o.rn % 2) AS Quantity,
    CAST(9.99 + (o.rn * 2) AS decimal(18,2)) AS UnitPrice,
    'USD' AS UnitPriceCurrency,
    CONCAT('Sample Book ', v.BookId) AS BookTitle,
    'Paperback' AS VariantName,
    GETUTCDATE() AS CreatedAt,
    NULL AS UpdatedAt
FROM OrderList o
JOIN VariantList v ON v.rn = o.rn;


/* ====================== PAYMENTS (1 mỗi order) ====================== */
INSERT INTO Payments (
    OrderId,
    Amount,
    AmountCurrency,
    Method,
    Status,
    TransactionId,
    PaidAt,
    CreatedAt,
    UpdatedAt
)
SELECT
    o.Id AS OrderId,
    o.FinalAmount AS Amount,
    o.FinalAmountCurrency AS AmountCurrency,
    CASE (o.Id % 4)
        WHEN 0 THEN 'CreditCard'
        WHEN 1 THEN 'EWallet'
        WHEN 2 THEN 'CashOnDelivery'
        ELSE 'BankTransfer'
    END AS Method,
    CASE o.Status
        WHEN 'Delivered'  THEN 'Completed'
        WHEN 'Shipped'    THEN 'Completed'
        WHEN 'Processing' THEN 'Pending'
        WHEN 'Pending'    THEN 'Pending'
        WHEN 'Cancelled'  THEN 'Refunded'
        WHEN 'Returned'   THEN 'Refunded'
        ELSE 'Pending'
    END AS Status,
    CONCAT('TXN', RIGHT('0000' + CAST(o.Id AS varchar(4)), 4)) AS TransactionId,
    CASE 
        WHEN o.Status IN ('Delivered','Shipped') THEN DATEADD(DAY, -1, GETUTCDATE())
        ELSE NULL
    END AS PaidAt,
    GETUTCDATE() AS CreatedAt,
    NULL AS UpdatedAt
FROM Orders o;


PRINT 'MockData_Domain.sql completed: Users, SellerProfiles, AddressWallets, Carts, CartItems, Orders, OrderItems, Payments.';

